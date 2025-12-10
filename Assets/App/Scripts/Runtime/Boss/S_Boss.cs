using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
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

    //[Header("Outputs")]

    [HideInInspector] public UnityEvent<float> onUpdateBossHealth = null;

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
    }

    private void OnEnable()
    {
        bossDetectionRange.onTargetDetected.AddListener(SetTarget);
    }
    private void OnDisable()
    {
        bossDetectionRange.onTargetDetected.RemoveListener(SetTarget);
    }
    private void Start()
    {
        UpdateState(S_EnumBossState.Idle);
        UpdatePhaseState(S_EnumBossPhaseState.Phase1);
    }
    
    private void Update()
    {
        if (target != null && (unlockRotate || !isAttacking) && !isDead) RotateEnemy();

        //if (isChasing) Chase();

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
                break;
            case S_EnumBossState.Chase:
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
        }
        else
        {
            if (health != maxHealth)
            {
                health = maxHealth;
                onUpdateBossHealth.Invoke(health);
            }

            aimPoint = null;
            detectionCollider.enabled = false;

            UpdateState(S_EnumBossState.Idle);
        }
    } 
    #endregion

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if(target != null) UpdateHealth(damage);

    }

    private void UpdateHealth(float damage)
    {
        health = Mathf.Max(health - damage, 0);
    }
}