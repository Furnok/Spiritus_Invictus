using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class S_Enemy : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Animations Parameters")]
    [SerializeField, S_AnimationName] private string moveParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string deathParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string idleAttack;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string attackParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string comboParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string stopAttackParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string hitHeavyParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string tBagParam;

    [TabGroup("Settings")]
    [Title("Mask")]
    [SerializeField] private LayerMask obstacleMask;

    [TabGroup("Settings")]
    [Title("Patrol Points")]
    [SerializeField] private List<GameObject> patrolPointsList;

    [TabGroup("References")]
    [Title("Agent")]
    [SerializeField] private BehaviorGraphAgent behaviorAgent;

    [TabGroup("References")]
    [SerializeField] private NavMeshAgent navMeshAgent;

    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider bodyCollider;

    [TabGroup("References")]
    [SerializeField] private Collider DetectionCollider;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [Title("Body")]
    [SerializeField] private GameObject body;

    [TabGroup("References")]
    [Title("Projectile")]
    [SerializeField] private GameObject spawnProjectilePoint;

    [TabGroup("References")]
    [SerializeField] private S_EnemyProjectile enemyProjectile;

    [TabGroup("References")]
    [Title("Scripts")]
    [SerializeField] private S_EnemyAttackData enemyAttackData;

    [TabGroup("References")]
    [SerializeField] private S_EnemyDetectionRange enemyDetectionRange;

    [TabGroup("References")]
    [SerializeField] private S_EnemyHurt enemyHurt;

    [TabGroup("References")]
    [SerializeField] private S_EnemyUI enemyUI;

    [TabGroup("References")]
    [SerializeField] private S_EnemyMaxTravelZone enemyMaxTravelZone;

    [TabGroup("References")]
    [Title("Patrol Points Parent")]
    [SerializeField] private Transform patrolPoints;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerDeath rseOnPlayerDeath;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnEnemyTargetDied rseOnEnemyTargetDied;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnGamePause rseOnGamePause;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_EnemyData ssoEnemyData;

    [HideInInspector] public UnityEvent<float> onUpdateEnemyHealth = null;
    [HideInInspector] public UnityEvent onGetHit = null;

    private float health = 0;
    private float maxhealth = 0;
    private bool isPaused = false;
    private int currentPatrolIndex = 0;
    private Transform aimPoint = null;
    private BlackboardVariable<bool> isPatroling = null;
    private BlackboardVariable<bool> isChasing = null;
    private BlackboardVariable<bool> isAttacking = null;
    private AnimatorOverrideController overrideController = null;
    private Coroutine comboCoroutine = null;
    private Coroutine resetCoroutine = null;
    private Coroutine patrolCoroutine = null;
    private bool isPerformingCombo = false;
    private S_EnumEnemyState? pendingState = null;
    private GameObject pendingTarget = null;
    private bool isPlayerDead = false;
    private GameObject target = null;
    private bool isChase = false;
    private bool isPatrolling = false;
    private bool lastMoveState = false;
    private bool isDead = false;
    private S_ClassAnimationsCombos combo;

    private void Awake()
    {
        Refresh();

        Animator anim = GetComponent<Animator>();
        AnimatorOverrideController instance = new AnimatorOverrideController(ssoEnemyData.Value.controllerOverride);

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        ssoEnemyData.Value.controllerOverride.GetOverrides(overrides);
        instance.ApplyOverrides(overrides);

        anim.runtimeAnimatorController = instance;

        overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

        behaviorAgent.SetVariableValue<GameObject>("Body", body);
        behaviorAgent.SetVariableValue<List<GameObject>>("PatrolPoints", patrolPointsList);
        behaviorAgent.SetVariableValue<Animator>("Animator", animator);
        behaviorAgent.SetVariableValue<Collider>("BodyCollider", bodyCollider);
        behaviorAgent.SetVariableValue<Collider>("DetectionRangeCollider", DetectionCollider);

        health = ssoEnemyData.Value.health;
        maxhealth = ssoEnemyData.Value.health;
        behaviorAgent.SetVariableValue<float>("Health", ssoEnemyData.Value.health);
        behaviorAgent.SetVariableValue<float>("StartPatrolWaitMin", ssoEnemyData.Value.startPatrolWaitMin);
        behaviorAgent.SetVariableValue<float>("StartPatrolWaitMax", ssoEnemyData.Value.startPatrolWaitMax);
        behaviorAgent.SetVariableValue<float>("TimeBeforeChaseMin", ssoEnemyData.Value.timeBeforeChaseMin);
        behaviorAgent.SetVariableValue<float>("TimeBeforeChaseMax", ssoEnemyData.Value.timeBeforeChaseMax);

        behaviorAgent.SetVariableValue<string>("MoveParam", moveParam);
        behaviorAgent.SetVariableValue<string>("DeathParam", deathParam);
        behaviorAgent.SetVariableValue<string>("AttackIdleParam", idleAttack);
        behaviorAgent.SetVariableValue<string>("AttackParam", attackParam);
        behaviorAgent.SetVariableValue<string>("HitHeavyParam", hitHeavyParam);

        enemyDetectionRange.Setup(ssoEnemyData);
        enemyUI.Setup(ssoEnemyData);
    }

    private void OnEnable()
    {
        rseOnGamePause.action += Pause;

        enemyDetectionRange.onTargetDetected.AddListener(SetTarget);
        enemyHurt.onUpdateEnemyHealth.AddListener(UpdateHealth);
        enemyMaxTravelZone.onTargetDetected.AddListener(SetTarget);

        rseOnPlayerDeath.action += PlayerDied;

        if (behaviorAgent.GetVariable("IsPatroling", out isPatroling))
        {
            isPatroling.OnValueChanged += Patroling;
        }

        if (behaviorAgent.GetVariable("IsChasing", out isChasing))
        {
            isChasing.OnValueChanged += Chasing;
        }

        if (behaviorAgent.GetVariable("IsAttacking", out isAttacking))
        {
            isAttacking.OnValueChanged += AttackCombo;
        }
    }

    private void OnDisable()
    {
        rseOnGamePause.action -= Pause;

        enemyDetectionRange.onTargetDetected.RemoveListener(SetTarget);
        enemyHurt.onUpdateEnemyHealth.RemoveListener(UpdateHealth);
        enemyMaxTravelZone.onTargetDetected.RemoveListener(SetTarget);

        rseOnPlayerDeath.action -= PlayerDied;

        if (isPatroling != null)
        {
            isPatroling.OnValueChanged -= Patroling;
        }

        if (isChasing != null)
        {
            isChasing.OnValueChanged -= Chasing;
        }

        if (isAttacking != null)
        {
            isAttacking.OnValueChanged -= AttackCombo;
        }

        overrideController["AttackAnimation"] = null;
        overrideController["AttackAnimation2"] = null;
    }

    private void Update()
    {
        if (isChase && target != null)
        {
            Chase();
        }
    }

    private void FixedUpdate()
    {
        if (!isPaused)
        {
            bool isMoving = navMeshAgent.velocity.magnitude > 0.2f;
            lastMoveState = isMoving;
            animator.SetBool(moveParam, isMoving);
        }
        else
        {
            animator.SetBool(moveParam, lastMoveState);
        }
    }

    [TabGroup("Settings")]
    [Button(ButtonSizes.Medium)]
    private void RefreshPatrolPoints()
    {
        Refresh();
    }

    private void Refresh()
    {
        patrolPointsList.Clear();

        foreach (Transform child in patrolPoints)
        {
            patrolPointsList.Add(child.gameObject);
        }
    }

    private void Chase()
    {
        float distance = Vector3.Distance(body.transform.position, target.transform.position);

        bool destinationReached = distance <= (combo.distanceToChase);

        if (!destinationReached)
        {
            navMeshAgent.SetDestination(target.transform.position);
        }
        else if (destinationReached)
        {
            isChase = false;

            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
            behaviorAgent.SetVariableValue("State", S_EnumEnemyState.Attack);
            behaviorAgent.Restart();
        }
    }

    private void Pause(bool value)
    {
        if (value)
        {
            isPaused = true;

            animator.speed = 0f;
            behaviorAgent.enabled = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.speed = 0;
        }
        else
        {
            StartCoroutine(S_Utils.Delay(0.05f, () => isPaused = false));

            animator.speed = 1f;
            behaviorAgent.enabled = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = ssoEnemyData.Value.speedChase;
        }
    }

    private void SetTarget(GameObject newTarget)
    {
        if (newTarget == target || isDead)
        {
            return;
        }

        target = newTarget;

        if (isPerformingCombo)
        {
            pendingTarget = newTarget;
            pendingState = (newTarget == null) ? S_EnumEnemyState.Patrol : S_EnumEnemyState.Chase;
            return;
        }

        behaviorAgent.SetVariableValue<GameObject>("Target", newTarget);

        if (newTarget != null)
        {
            newTarget.TryGetComponent<IAimPointProvider>(out IAimPointProvider aimPointProvider);
            aimPoint = aimPointProvider != null ? aimPointProvider.GetAimPoint() : newTarget.transform;

            float distance = Vector3.Distance(transform.position, newTarget.transform.position);
            Vector3 dir = (newTarget.transform.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distance, obstacleMask))
            {
                Debug.DrawLine(transform.position, newTarget.transform.position, Color.yellow);

                if (hit.collider.gameObject != newTarget)
                {
                    behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Patrol);
                }
                else
                {
                    behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Chase);
                }
            }
            else
            {
                behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Chase);
            }
        }
        else
        {
            aimPoint = null;

            behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Patrol);
        }

        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        behaviorAgent.Restart();
    }

    private void UpdateHealth(float damage)
    {
        health = Mathf.Max(health - damage, 0);
        onUpdateEnemyHealth.Invoke(health);

        behaviorAgent.SetVariableValue<float>("Health", health);
        onGetHit.Invoke();

        if (damage >= maxhealth / 2)
        {
            enemyAttackData.DisableWeaponCollider();

            StopAllCoroutines();
            comboCoroutine = null;
            resetCoroutine = null;
            patrolCoroutine = null;

            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;

            isPerformingCombo = false;

            behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.HeavyHit);
            behaviorAgent.Restart();

            resetCoroutine = StartCoroutine(ResetAttack());
        }

        if (health <= 0)
        {
            isDead = true;

            enemyAttackData.DisableWeaponCollider();

            StopAllCoroutines();
            comboCoroutine = null;
            resetCoroutine = null;
            patrolCoroutine = null;

            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;

            behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Death);
            behaviorAgent.Restart();

            rseOnEnemyTargetDied.Call(body);
        }
        else if (target == null)
        {
            behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Chase);
            behaviorAgent.Restart();
        }
    }

    private void TBag()
    {
        if (isPlayerDead)
        {
            float rnd = Random.Range(0f, 100f);
            float chance = ssoEnemyData.Value.chanceForEasterEgg;

            if (rnd < chance)
            {
                animator.SetTrigger(tBagParam);

                behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Idle);
                behaviorAgent.Restart();
            }
            else
            {
                animator.SetBool(idleAttack, false);

                behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Idle);
                behaviorAgent.Restart();
            }

            isPlayerDead = false;
        }
    }

    private void PlayerDied()
    {
        if (target == null)
        {
            return;
        }

        isPlayerDead = true;

        if (isPerformingCombo)
        {
            pendingState = S_EnumEnemyState.Idle;
            return;
        }

        TBag();
    }

    public void Patroling()
    {
        if (isPatroling != null && isPatroling.Value)
        {
            if (patrolPointsList == null || patrolPointsList.Count == 0)
                return;

            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
            }

            navMeshAgent.speed = ssoEnemyData.Value.speedPatrol;
            patrolCoroutine = StartCoroutine(PatrolRoutine());
        }
        else
        {
            isPatrolling = false;
        }
    }

    private IEnumerator PatrolRoutine()
    {
        isPatrolling = true;

        while (isPatrolling)
        {
            GameObject targetPoint = patrolPointsList[currentPatrolIndex];

            if (targetPoint == null)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPointsList.Count;
                yield return null;
                continue;
            }

            navMeshAgent.SetDestination(targetPoint.transform.position);

            while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            float waitTime = Random.Range(ssoEnemyData.Value.patrolPointWaitMin, ssoEnemyData.Value.patrolPointWaitMax);

            yield return WaitForSecondsWhileUnpaused(waitTime);

            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPointsList.Count;
        }
    }

    public void Chasing()
    {
        if (isChasing != null && isChasing.Value)
        {
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
            }

            GetCombo();
            navMeshAgent.speed = ssoEnemyData.Value.speedChase;
            isChase = true;
        }
        else
        {
            isChase = false;
        }
    }

    private void GetCombo()
    {
        int rnd = Random.Range(0, ssoEnemyData.Value.listCombos.Count);

        combo = ssoEnemyData.Value.listCombos[rnd];

        behaviorAgent.SetVariableValue<float>("DistanceToLoseAttack", combo.distanceToLoseAttack);
    }

    public void AttackCombo()
    {
        if (isAttacking != null && isAttacking.Value)
        {
            comboCoroutine = StartCoroutine(PlayComboSequence());
        }
    }

    private IEnumerator WaitForSecondsWhileUnpaused(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (!isPaused)
                timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator PlayComboSequence()
    {
        yield return new WaitForSeconds(0.4f);

        isPerformingCombo = true;

        for (int i = 0; i < combo.listAnimationsCombos.Count; i++)
        {
            string overrideKey = (i % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
            overrideController[overrideKey] = combo.listAnimationsCombos[i].animation;

            enemyAttackData.SetAttackMode(combo.listAnimationsCombos[i].attackData);
            animator.SetTrigger(i == 0 ? attackParam : comboParam);

            if (combo.listAnimationsCombos[i].attackData.attackType == S_EnumEnemyAttackType.Projectile)
            {
                S_EnemyProjectile projectileInstance = Instantiate(enemyProjectile, spawnProjectilePoint.transform.position, Quaternion.identity);
                projectileInstance.Initialize(bodyCollider.transform, aimPoint, combo.listAnimationsCombos[i].attackData);
            }

            yield return WaitForSecondsWhileUnpaused(combo.listAnimationsCombos[i].animation.length);
        }

        isAttacking.Value = false;
        comboCoroutine = null;
        animator.SetTrigger(stopAttackParam);

        isPerformingCombo = false;

        if (pendingState.HasValue)
        {
            animator.SetBool(idleAttack, false);

            TBag();

            behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", pendingState.Value);
            behaviorAgent.Restart();

            pendingState = null;
        }

        if (pendingTarget != null)
        {
            SetTarget(pendingTarget);
            pendingTarget = null;
        }

        GetCombo();
        resetCoroutine = StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return WaitForSecondsWhileUnpaused(ssoEnemyData.Value.attackCooldown);

        behaviorAgent.SetVariableValue<bool>("CanAttack", true);
    }

    private void OnDrawGizmos()
    {
        if (patrolPointsList == null || patrolPointsList.Count < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < patrolPointsList.Count; i++)
        {
            GameObject current = patrolPointsList[i];
            GameObject next = patrolPointsList[(i + 1) % patrolPointsList.Count];

            if (current != null && next != null)
            {
                Gizmos.DrawLine(current.transform.position, next.transform.position);
                Gizmos.DrawSphere(current.transform.position, 0.2f);
            }
        }

        if (Application.isPlaying && patrolPointsList.Count > 0)
        {
            if (isPatrolling && currentPatrolIndex >= 0 && currentPatrolIndex < patrolPointsList.Count)
            {
                GameObject target = patrolPointsList[currentPatrolIndex];
                if (target != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(target.transform.position, 0.3f);
                }
            }
        }
    }
}