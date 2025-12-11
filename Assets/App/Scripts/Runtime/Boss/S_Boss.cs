using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class S_Boss : MonoBehaviour
{
    #region Variables
    [TabGroup("Settings")]
    [Header("Settings")]
    [SerializeField] private SSO_BossData ssoBossData;

    [TabGroup("References")]
    [Title("Agent")]
    [SerializeField] private NavMeshAgent navMeshAgent;

    [TabGroup("References")]
    [Title("Body")]
    [SerializeField] private GameObject body;

    [TabGroup("References")]
    [Title("Center")]
    [SerializeField] private GameObject center;

    [TabGroup("References")]
    [Title("RigidBody")]
    [SerializeField] private Rigidbody rb;

    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider bodyCollider;

    [TabGroup("References")]
    [SerializeField] private Collider detectionCollider;

    [TabGroup("References")]
    [SerializeField] private Collider hurtCollider;

    [TabGroup("References")]
    [Title("Scripts")]
    [SerializeField] private S_BossDetectionRange bossDetectionRange;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [Title("Animation Parameters")]
    [SerializeField, S_AnimationName("animator")] private string moveParam;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string deathParam;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string attackParam;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string stunParam;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerGettingHit rseOnPlayerGettingHit;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerDeath rseOnPlayerDeath;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnExecuteAttack onExecuteAttack;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnEnemyTargetDied rseOnEnemyTargetDied;

    private List<S_ClassAttackOwned> listAttackOwneds = new();
    private List<S_ClassAttackOwned> listAttackOwnedPossibilities = new();
    private S_EnumBossState currentState = S_EnumBossState.Idle;
    private S_EnumBossPhaseState currentPhaseState = S_EnumBossPhaseState.Phase1;

    private AnimatorOverrideController overrideController = null;
    private GameObject target = null;
    private Transform aimPoint = null;

    private S_ClassAttackOwned lastAttack = null;
    private S_ClassAttackOwned currentAttack = null;

    private Coroutine idleCoroutine = null;
    private Coroutine comboCoroutine = null;
    private Coroutine stunCoroutine = null;

    private float health = 0;
    private float maxHealth = 0;
    private float lastValueHealth = 0;
    private float bossDifficultyLevel = 0;
    private float initDistance = 0;
    private float nextChangeTime = 0f;
    private int strafeDirection = 1;

    private bool isPerformingCombo = false;
    private S_EnumBossState? pendingState = null;
    private GameObject pendingTarget = null;

    private bool isDead = false;
    private bool isChasing = false;
    private bool isFighting = false;
    private bool isStunned = false;

    private bool canAttack = false;
    private bool isPlayerDeath = false;
    private bool isAttacking = false;
    private bool isStrafe = false;
    private bool isWaiting = false;
    private bool unlockRotate = false;

    private bool lastMoveState = false;
    private bool canChooseAttack = false; 
    #endregion

    private void Awake()
    {
        canAttack = true;
        canChooseAttack = true;
        navMeshAgent.avoidancePriority = Random.Range(0, 99);

        Animator anim = animator;
        AnimatorOverrideController instance = new AnimatorOverrideController(ssoBossData.Value.controllerOverride);

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        ssoBossData.Value.controllerOverride.GetOverrides(overrides);
        instance.ApplyOverrides(overrides);

        anim.runtimeAnimatorController = instance;

        overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        lastValueHealth = 101f;

        health = ssoBossData.Value.healthPhase1;
        maxHealth = ssoBossData.Value.healthPhase1;
        navMeshAgent.speed = ssoBossData.Value.walkSpeed;
        currentPhaseState = S_EnumBossPhaseState.Phase1;

        foreach (var bossAttack in ssoBossData.Value.listAttackPhase1)
        {
            var attackData = new S_ClassAttackOwned
            {
                bossAttack = bossAttack,
                frequency = 0,
                score = 0,
            };

            listAttackOwneds.Add(attackData);
        }

        UpdateLastHealthValue();
    }

    private void OnEnable()
    {
        bossDetectionRange.onTargetDetected.AddListener(SetTarget);
        rseOnPlayerGettingHit.action += LoseDifficultyLevel;
    }
    private void OnDisable()
    {
        bossDetectionRange.onTargetDetected.RemoveListener(SetTarget);
        rseOnPlayerGettingHit.action -= LoseDifficultyLevel;
    }
    private void Start()
    {
        UpdateLastHealthValue();
        UpdateState(S_EnumBossState.Idle);
        UpdatePhaseState(S_EnumBossPhaseState.Phase1);
        bossDifficultyLevel = ssoBossData.Value.initialBossDifficultyLevel;
    }
    
    private void Update()
    {
        if (target != null && (unlockRotate || !isAttacking) && !isDead) RotateEnemy();

        if (isChasing) Chase();

        //if (isFighting) Fight();
    }
    private void FixedUpdate()
    {
        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool(moveParam, isMoving);
    }
    public void RotateEnemy()
    {
        Vector3 direction = target.transform.position - center.transform.position;
        direction.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(direction);

        transform.DOKill();
        transform.DORotateQuaternion(targetRot, ssoBossData.Value.rotationTime);
    }

    public void RotateEnemyAnim()
    {
        unlockRotate = true;
    }

    public void StopRotateEnemyAnim()
    {
        unlockRotate = false;
    }

    #region States
    private void UpdateState(S_EnumBossState newState)
    {
        ResetBoss();
        currentState = newState;

        switch (currentState)
        {
            case S_EnumBossState.Idle:
                Idle();
                break;
            case S_EnumBossState.Chase:
                Chasing();
                break;
            case S_EnumBossState.Combat:
                break;
            case S_EnumBossState.Stun:
                break;
            case S_EnumBossState.Death:
                break;
        }
    }
    private void UpdatePhaseState(S_EnumBossPhaseState newPhaseState)
    {
        currentPhaseState = newPhaseState;

        switch (currentPhaseState)
        {
            case S_EnumBossPhaseState.Phase1:
                break;
            case S_EnumBossPhaseState.Phase2:
                break;
        }
    }

    private void ResetBoss()
    {
        isPerformingCombo = false;
        isChasing = false;
        isFighting = false;
        isStunned = false;
        isStrafe = false;
        isAttacking = false;
        isWaiting = false;
        unlockRotate = false;

        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }
    #endregion

    #region Target
    private void SetTarget(GameObject newTarget)
    {
        if (newTarget == target || isDead) return;

        target = newTarget;

        if (isPerformingCombo || isStunned)
        {
            pendingTarget = target;
            pendingState = (target == null) ? S_EnumBossState.Idle : S_EnumBossState.Chase;
            return;
        }

        if (target != null)
        {
            newTarget.TryGetComponent<I_AimPointProvider>(out I_AimPointProvider aimPointProvider);
            aimPoint = aimPointProvider != null ? aimPointProvider.GetAimPoint() : newTarget.transform;

            UpdateState(S_EnumBossState.Chase);
            StartCoroutine(GainDifficultyLevel());
        }
        else
        {
            if (health != maxHealth)
            {
                health = maxHealth;
            }

            aimPoint = null;
            detectionCollider.enabled = false;

            UpdateState(S_EnumBossState.Idle);
        }
    }
    #endregion

    #region Chase
    private void Chasing()
    {
        isChasing = true;

        ChooseAttack();

        navMeshAgent.speed = ssoBossData.Value.walkSpeed;
        initDistance = Vector3.Distance(body.transform.position, target.transform.position);
    }

    private void Chase()
    {
        float distanceToTarget = Vector3.Distance(body.transform.position, target.transform.position);
        bool destinationReached = distanceToTarget <= (ssoBossData.Value.distanceToChase);
        if (!destinationReached)
        {
            navMeshAgent.SetDestination(target.transform.position);

            if (distanceToTarget <= initDistance * (ssoBossData.Value.distanceToRun / 100f))
            {
                if (canChooseAttack)
                {
                    ChooseAttack();
                    if (currentAttack.bossAttack.isAttackDistance)
                    {
                        navMeshAgent.ResetPath();
                        navMeshAgent.velocity = Vector3.zero;
                        isChasing = false;
                        UpdateState(S_EnumBossState.Combat);
                        ExecuteAttack(currentAttack);
                    }
                    else
                    {
                        navMeshAgent.speed = ssoBossData.Value.runSpeed;
                        animator.SetFloat("MoveSpeed", ssoBossData.Value.runSpeed);
                    }
                }
            }
        }
        else if (destinationReached)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
            isChasing = false;
            UpdateState(S_EnumBossState.Combat);
            ExecuteAttack(currentAttack);
        }
    }
    #endregion

    #region Idle
    private void Idle()
    {
        if (target != null && isChasing)
        {
            UpdateState(S_EnumBossState.Chase);
        }
    } 
    #endregion

    #region Health/Death
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (target != null) UpdateHealth(damage);

    }

    private void UpdateHealth(float damage)
    {
        health = Mathf.Max(health - damage, 0);

        UpdateLastHealthValue();

        if (health <= 0)
        {
            if (currentPhaseState == S_EnumBossPhaseState.Phase1)
            {
                UpdateState(S_EnumBossState.Death);
                DeathPhase1();
            }
            else
            {
                UpdateState(S_EnumBossState.Death);
                DeathPhase2();
            }
        }
    }
    private void UpdateLastHealthValue()
    {
        var minValue = (health / maxHealth) * 100;

        SetListAttackPossible(minValue, lastValueHealth);

        lastValueHealth = minValue;
    }
    private void DeathPhase1()
    {
        StopAllCoroutines();

        ResetBoss();
        canChooseAttack = false;
        currentAttack = null;

        listAttackOwneds.Clear();
        listAttackOwnedPossibilities.Clear();

        currentPhaseState = S_EnumBossPhaseState.Phase2;
        currentState = S_EnumBossState.Idle;
        health = ssoBossData.Value.healthPhase2;
        maxHealth = ssoBossData.Value.healthPhase2;
        lastValueHealth = 101f;

        foreach (var bossAttack in ssoBossData.Value.listAttackPhase2)
        {
            var attackData = new S_ClassAttackOwned
            {
                bossAttack = bossAttack,
                frequency = 0,
                score = 0,
            };
            listAttackOwneds.Add(attackData);
        }

        UpdateLastHealthValue();

        canChooseAttack = true;
    }
    private void DeathPhase2()
    {
        isDead = true;
        animator.SetTrigger(deathParam);

        canAttack = false;
        canChooseAttack = false;
        target = null;

        bodyCollider.enabled = false;
        detectionCollider.enabled = false;
        hurtCollider.enabled = false;

    }
    #endregion

    #region Difficulty
    private void LoseDifficultyLevel()
    {
        bossDifficultyLevel -= ssoBossData.Value.difficultyLoseWhenPlayerHit;
        bossDifficultyLevel = Mathf.Clamp(bossDifficultyLevel, 0, ssoBossData.Value.maxDifficultyLevel);
    }

    private IEnumerator GainDifficultyLevel()
    {
        bossDifficultyLevel += ssoBossData.Value.difficultyGainPerSecond;
        bossDifficultyLevel = Mathf.Clamp(bossDifficultyLevel, 0, ssoBossData.Value.maxDifficultyLevel);

        yield return new WaitForSeconds(1);

        StartCoroutine(GainDifficultyLevel());
    }
    #endregion

    #region Attack
    private void AddListAttackPossible(S_ClassAttackOwned bossAttack)
    {
        listAttackOwnedPossibilities.Add(bossAttack);
    }

    private void SetListAttackPossible(float minValue, float maxValue)
    {
        foreach (var attack in listAttackOwneds)
        {
            if (attack.bossAttack.pvBossUnlock >= minValue && attack.bossAttack.pvBossUnlock < maxValue) AddListAttackPossible(attack);
        }
    }

    private void ChooseAttack()
    {
        canChooseAttack = false;
        isStrafe = false;

        var minAttackFrequency = listAttackOwnedPossibilities.Min(a => a.frequency);
        int roundDifficulty = Mathf.RoundToInt(bossDifficultyLevel);

        foreach (var attack in listAttackOwnedPossibilities)
        {
            if (attack.bossAttack.difficultyLevel == roundDifficulty) attack.score += difficultyScore;

            if (attack.frequency == minAttackFrequency) attack.score += frequencyScore;

            if (lastAttack == null) continue;

            if (attack.bossAttack.listComboData[0].attackData.attackType != lastAttack.bossAttack.listComboData[^1].attackData.attackType) attack.score += synergieScore;
        }

        var maxScore = listAttackOwnedPossibilities.Max(a => a.score);

        var bestAttacks = listAttackOwnedPossibilities
            .Where(a => a.score == maxScore)
            .ToList();

        var chosenAttack = bestAttacks[Random.Range(0, bestAttacks.Count)];

        currentAttack = chosenAttack;

        foreach (var attack in listAttackOwnedPossibilities) attack.score = 0;
    }
    private void ExecuteAttack(S_ClassAttackOwned attack)
    {
        lastAttack = attack;
        attack.frequency++;

        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        onExecuteAttack.Call(attack.bossAttack);
    }

    private IEnumerator TimeForChooseAttack()
    {
        float rndTime = Random.Range(ssoBossData.Value.minTimeChooseAttack, ssoBossData.Value.maxTimeChooseAttack);

        yield return new WaitForSeconds(rndTime);

        ChooseAttack();

        ExecuteAttack(currentAttack);
    } 
    #endregion
}