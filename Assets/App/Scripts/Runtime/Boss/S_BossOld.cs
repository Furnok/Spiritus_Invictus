using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class S_BossOld : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Boss")]
    [SerializeField] private float bossDifficultyLevel;

    [TabGroup("Settings")]
    [SerializeField] private float maxDifficultyLevel;

    [TabGroup("Settings")]
    [SerializeField] private float difficultyGainPerSecond;

    [TabGroup("Settings")]
    [SerializeField] private float difficultyLoseWhenPlayerHit;

    [TabGroup("Settings")]
    [SerializeField] private float difficultyScore;

    [TabGroup("Settings")]
    [SerializeField] private float frequencyScore;

    [TabGroup("Settings")]
    [SerializeField] private float synergieScore;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float minTimeChooseAttack;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float maxTimeChooseAttack;

    [TabGroup("Settings")]
    [Title("Strafe")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float strafeChangeInterval = 1.5f;

    [TabGroup("Settings")]
    [SerializeField] private float strafeRadius = 5f;

    [TabGroup("Settings")]
    [SerializeField] private float strafeDistance = 2f;

    [TabGroup("Settings")]
    [SerializeField] private float rotationSpeed = 6f;

    [TabGroup("References")]
    [Title("Agent")]
    [SerializeField] private BehaviorGraphAgent behaviorAgent;

    [TabGroup("References")]
    [SerializeField] private NavMeshAgent navMeshAgent;

    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider bodyCollider;

    [TabGroup("References")]
    [SerializeField] private Collider detectionCollider;

    [TabGroup("References")]
    [SerializeField] private Collider hurtCollider;

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

    [TabGroup("References")]
    [Title("Body")]
    [SerializeField] private GameObject body;

    [TabGroup("References")]
    [Title("Scripts")]
    [SerializeField] private S_BossDetectionRange bossDetectionRange;

    [TabGroup("References")]
    [SerializeField] private S_BossHurt bossHurt;

    [TabGroup("References")]
    [SerializeField] private S_BossAttack bossAttack;

    [TabGroup("References")]
    [Title("Mask")]
    [SerializeField] private LayerMask obstacleMask;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerGettingHit rseOnPlayerGettingHit;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerDeath rseOnPlayerDeath;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnExecuteAttack onExecuteAttack;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnEnemyTargetDied rseOnEnemyTargetDied;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_BossData ssoBossData;

    [HideInInspector] public UnityEvent<float> onUpdateBossHealth = null;
    [HideInInspector] public UnityEvent onGetHit = null;

    private List<S_ClassAttackOwned> listAttackOwneds = new();
    private List<S_ClassAttackOwned> listAttackOwnedPossibilities = new();
    private S_EnumBossPhaseState currentPhaseState = S_EnumBossPhaseState.Phase1;

    private float health = 0;
    private float maxHealth = 0;
    private float lastValueHealth = 0;
    private float initDistance = 0;

    private GameObject target = null;
    private Transform aimPoint = null;
    private BlackboardVariable<bool> isChasing = null;

    private bool isDead = false;
    private bool isChase = false;
    private bool isStrafe = false;
    private bool lastMoveState = false;
    private bool canChooseAttack = false;
    
    private S_ClassAttackOwned lastAttack = null;
    private S_ClassAttackOwned currentAttack = null;

    private float nextChangeTime = 0f;
    private int strafeDirection = 1;

    private Coroutine cooldowAttackCoroutine = null;
    private Coroutine timeChooseAttackCoroutine = null;

    private void Awake()
    {
        lastValueHealth = 101f;

        health = ssoBossData.Value.healthPhase1;
        maxHealth = ssoBossData.Value.healthPhase1;
        navMeshAgent.speed = ssoBossData.Value.walkSpeed;
        currentPhaseState = S_EnumBossPhaseState.Phase1;

        behaviorAgent.SetVariableValue<S_EnumBossPhaseState>("PhaseState", S_EnumBossPhaseState.Phase1);
        behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Idle);
        behaviorAgent.SetVariableValue<GameObject>("Body", body);
        behaviorAgent.SetVariableValue<Animator>("Animator", animator);
        behaviorAgent.SetVariableValue<float>("Health", health);
        behaviorAgent.SetVariableValue<Collider>("BodyCollider", bodyCollider);
        behaviorAgent.SetVariableValue<Collider>("DetectionCollider", detectionCollider);
        behaviorAgent.SetVariableValue<Collider>("HurtBox", hurtCollider);
        behaviorAgent.SetVariableValue<string>("DeathParam", deathParam);
        behaviorAgent.SetVariableValue<string>("MoveParam", moveParam);
        behaviorAgent.SetVariableValue<string>("StunParam", stunParam);
        behaviorAgent.SetVariableValue<string>("AttackParam", attackParam);

        foreach ( var bossAttack in ssoBossData.Value.listAttackPhase1)
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
        bossHurt.onUpdateEnemyHealth.AddListener(UpdateHealth);
        rseOnPlayerGettingHit.action += LoseDifficultyLevel;

        if (behaviorAgent.GetVariable("IsChasing", out isChasing)) isChasing.OnValueChanged += Chasing;
    }

    private void OnDisable()
    {
        bossDetectionRange.onTargetDetected.RemoveListener(SetTarget);
        bossHurt.onUpdateEnemyHealth.RemoveListener(UpdateHealth);
        rseOnPlayerGettingHit.action -= LoseDifficultyLevel;

        if (isChasing != null) isChasing.OnValueChanged -= Chasing;
    }

    private void Start()
    {
        Debug.Log(listAttackOwnedPossibilities.Count);
    }

    private void Update()
    {
        Debug.Log(isStrafe);

        if (isChase && target != null) Chase();

        if (isStrafe && target != null) Strafing();
    }

    private void FixedUpdate()
    {
        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
        lastMoveState = isMoving;
        animator.SetBool(moveParam, isMoving);
    }

    private void SetTarget(GameObject newTarget)
    {
        if (newTarget == target || isDead) return;

        target = newTarget;

        behaviorAgent.SetVariableValue<GameObject>("Target", newTarget);

        if (target != null)
        {
            newTarget.TryGetComponent<I_AimPointProvider>(out I_AimPointProvider aimPointProvider);
            aimPoint = aimPointProvider != null ? aimPointProvider.GetAimPoint() : newTarget.transform;
            bossAttack.aimPointPlayer = aimPoint;

            float distance = Vector3.Distance(transform.position, newTarget.transform.position);
            Vector3 dir = (newTarget.transform.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distance, obstacleMask))
            {
                Debug.DrawLine(transform.position, newTarget.transform.position, Color.yellow);

                if (hit.collider.gameObject != newTarget)
                {
                    if (health != maxHealth)
                    {
                        health = maxHealth;
                        onUpdateBossHealth.Invoke(health);
                    }

                    behaviorAgent.SetVariableValue<float>("Health", health);

                    behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Idle);
                }
                else
                {
                    behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Chase);
                    canChooseAttack = true;

                    StartCoroutine(GainDifficultyLevel());
                }
            }
            else
            {
                behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Chase);
                canChooseAttack = true;

                StartCoroutine(GainDifficultyLevel());
            }
        }
        else
        {
            if (health != maxHealth)
            {
                health = maxHealth;
                onUpdateBossHealth.Invoke(health);
            }

            behaviorAgent.SetVariableValue<float>("Health", health);

            aimPoint = null;

            detectionCollider.enabled = false;

            behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Idle);
        }

        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;
    }

    #region Chase
    private void Chase()
    {
        Debug.Log("Chase");

        float distance = Vector3.Distance(body.transform.position, target.transform.position);
        bool destinationReached = distance <= (ssoBossData.Value.distanceToChase);

        if (!destinationReached)
        {
            navMeshAgent.SetDestination(target.transform.position);

            if (distance <= initDistance * (ssoBossData.Value.distanceToRun / 100))
            {
                if (canChooseAttack)
                {
                    ChooseAttack();

                    if (currentAttack.bossAttack.isAttackDistance == true)
                    {
                        Debug.Log("Attack Distance");

                        navMeshAgent.ResetPath();
                        navMeshAgent.velocity = Vector3.zero;
                        behaviorAgent.SetVariableValue("State", S_EnumBossState.Combat);

                        if (cooldowAttackCoroutine != null)
                        {
                            StopCoroutine(cooldowAttackCoroutine);
                            cooldowAttackCoroutine = null;
                        }

                        cooldowAttackCoroutine = StartCoroutine(CooldownAttack(currentAttack));
                        behaviorAgent.Restart();
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
            isChase = false;

            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
            behaviorAgent.SetVariableValue("State", S_EnumBossState.Combat);

            if (cooldowAttackCoroutine != null)
            {
                StopCoroutine(cooldowAttackCoroutine);
                cooldowAttackCoroutine = null;
            }

            cooldowAttackCoroutine = StartCoroutine(CooldownAttack(currentAttack));
            behaviorAgent.Restart();
        }
    }

    public void Chasing()
    {
        if (isChasing != null && isChasing.Value)
        {
            isStrafe = false;
            navMeshAgent.speed = ssoBossData.Value.walkSpeed;
            animator.SetFloat("MoveSpeed", ssoBossData.Value.walkSpeed);
            initDistance = Vector3.Distance(body.transform.position, target.transform.position);
            isChase = true;
        }
        else
        {
            isChase = false;
        }
    }

    private void Strafing()
    {
        if (target == null) return;

        if (canChooseAttack)
        {
            canChooseAttack = false;
            if(timeChooseAttackCoroutine != null)
            {
                StopCoroutine(timeChooseAttackCoroutine);
                timeChooseAttackCoroutine = null;
            }
            timeChooseAttackCoroutine = StartCoroutine(TimeForChooseAttack());
        }
        
        if (Time.time >= nextChangeTime)
        {
            strafeDirection *= -1;
            nextChangeTime = Time.time + strafeChangeInterval;
        }

        
        Vector3 offsetPlayer = transform.position - target.transform.position;
        offsetPlayer.y = 0;

        
        if (offsetPlayer.sqrMagnitude < 0.01f)
            offsetPlayer = transform.right * 1f;

        
        Vector3 offsetAtRadius = offsetPlayer.normalized * strafeRadius;

        
        Vector3 dir = Vector3.Cross(offsetAtRadius, Vector3.up).normalized * strafeDirection;

        
        Vector3 finalPos = target.transform.position + offsetAtRadius + dir * strafeDistance;

        
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = ssoBossData.Value.walkSpeed;
        navMeshAgent.SetDestination(finalPos);

        
        Vector3 lookPos = target.transform.position - transform.position;
        lookPos.y = 0;
        if (lookPos.sqrMagnitude > 0.01f)
        {
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }

        animator.SetFloat("MoveSpeed", ssoBossData.Value.walkSpeed);
    }
    #endregion

    #region Health
    private void UpdateHealth(float damage)
    {
        health = Mathf.Max(health - damage, 0);
        onUpdateBossHealth.Invoke(health);

        behaviorAgent.SetVariableValue<float>("Health", health);
        onGetHit.Invoke();

        UpdateLastHealthValue();

        if (health <= 0)
        {
            if (currentPhaseState == S_EnumBossPhaseState.Phase1)
            {
                StopAllCoroutines();
                cooldowAttackCoroutine = null;
                timeChooseAttackCoroutine = null;

                StartCoroutine(ChangeBossPhase());
            }
            else
            {
                Debug.Log("Phase 2 Finish: Boss Dead");

                isDead = true;
                isStrafe = false;
                canChooseAttack = false;

                StopAllCoroutines();
                cooldowAttackCoroutine = null;
                timeChooseAttackCoroutine = null;

                navMeshAgent.ResetPath();
                navMeshAgent.velocity = Vector3.zero;

                animator.SetTrigger(deathParam);

                rseOnEnemyTargetDied.Call(body);

                behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Death);
                behaviorAgent.Restart();
            }
        }
    }

    private void UpdateLastHealthValue()
    {
        var minValue = (health / maxHealth) * 100;

        SetListAttackPossible(minValue, lastValueHealth);

        lastValueHealth = minValue;
    }

    private IEnumerator ChangeBossPhase()
    {
        Debug.Log("Phase 1 Finish");

        isStrafe = false;
        canChooseAttack = false;
        currentAttack = null;

        listAttackOwneds.Clear();
        listAttackOwnedPossibilities.Clear();

        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        yield return new WaitForSeconds(5f);

        behaviorAgent.SetVariableValue<S_EnumBossPhaseState>("PhaseState", S_EnumBossPhaseState.Phase2);
        currentPhaseState = S_EnumBossPhaseState.Phase2;
        health = ssoBossData.Value.healthPhase2;
        maxHealth = ssoBossData.Value.healthPhase2;
        behaviorAgent.SetVariableValue<float>("Health", health);
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

        behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Chase);
        behaviorAgent.Restart();
    }
    #endregion

    #region Difficulty
    private void LoseDifficultyLevel()
    {
        bossDifficultyLevel -= difficultyLoseWhenPlayerHit;
        bossDifficultyLevel = Mathf.Clamp(bossDifficultyLevel, 0,maxDifficultyLevel);
    }

    private IEnumerator GainDifficultyLevel()
    {
        bossDifficultyLevel += difficultyGainPerSecond;
        bossDifficultyLevel = Mathf.Clamp(bossDifficultyLevel, 0, maxDifficultyLevel);

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
        Debug.Log("Choose Attack");

        canChooseAttack = false;
        isStrafe = false;

        var minAttackFrequency = listAttackOwnedPossibilities.Min(a => a.frequency);
        int roundDifficulty = Mathf.RoundToInt(bossDifficultyLevel);

        foreach (var attack in listAttackOwnedPossibilities)
        {
            if (attack.bossAttack.difficultyLevel == roundDifficulty)  attack.score+= difficultyScore;

            if (attack.frequency == minAttackFrequency) attack.score += frequencyScore;

            if (lastAttack == null) continue;

            if (attack.bossAttack.listComboData[0].attackData.attackType != lastAttack.bossAttack.listComboData[^1].attackData.attackType) attack.score+= synergieScore;
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

    private IEnumerator CooldownAttack(S_ClassAttackOwned attack)
    {
        ExecuteAttack(attack);

        Debug.Log("Execute");

        yield return new WaitForSeconds(attack.bossAttack.attackTime);

        Debug.Log("CD Finish");

        navMeshAgent.isStopped = false;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;

        navMeshAgent.ResetPath();

        canChooseAttack = true;
        isStrafe = true;
    }

    private IEnumerator TimeForChooseAttack()
    {
        float rndTime = Random.Range(minTimeChooseAttack, maxTimeChooseAttack);

        Debug.Log("Random choose");

        yield return new WaitForSeconds(rndTime);

        Debug.Log("CanChoose");

        ChooseAttack();

        if (cooldowAttackCoroutine != null)
        {
            StopCoroutine(cooldowAttackCoroutine);
            cooldowAttackCoroutine = null;
        }

        cooldowAttackCoroutine = StartCoroutine(CooldownAttack(currentAttack));
    }
    #endregion
}