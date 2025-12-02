using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class S_Enemy : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Animations Parameters")]
    [SerializeField, S_AnimationName("animator")] private string moveParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName("animator")] private string deathParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName("animator")] private string idleAttack;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName("animator")] private string attackParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName("animator")] private string comboParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName("animator")] private string stopAttackParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName("animator")] private string hitHeavyParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName("animator")] private string tBagParam;

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
    [SerializeField] private Collider detectionCollider;

    [TabGroup("References")]
    [SerializeField] private Collider hurtCollider;

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
    [SerializeField] private RSE_OnDataLoad rseOnDataLoad;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSendConsoleMessage rseOnSendConsoleMessage;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerRespawn rseOnPlayerRespawn;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_DataSaved rsoDataSaved;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_EnemyData ssoEnemyData;

    [HideInInspector] public UnityEvent<float> onUpdateEnemyHealth = null;
    [HideInInspector] public UnityEvent onGetHit = null;

    private float health = 0;
    private float maxhealth = 0;

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

    private GameObject targetInZone = null;
    private GameObject target = null;

    private bool isChase = false;
    private bool isPatrolling = false;
    private bool isDead = false;
    private bool isStrafe = false;
    private bool canAttack = false;

    private S_ClassAnimationsCombos combo = null;


    [SerializeField] private float strafeMinInterval = 1.0f;
    [SerializeField] private float strafeMaxInterval = 2.0f;
    [SerializeField] private float strafeWaitTime = 1f;         // time to wait after reaching a strafe point
    [SerializeField] private float strafeOffset = 2f;             // how far left/right from the circle
    [SerializeField] private float stoppingDistanceThreshold = 0.3f;

    private float nextChangeTime = 0f;
    private float nextWaitEndTime = 0f;
    private int strafeDirection = 1; // +1 = right, -1 = left
    private bool waiting = false;

    private void Awake()
    {
        Refresh();

        Animator anim = animator;
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
        behaviorAgent.SetVariableValue<Collider>("DetectionRangeCollider", detectionCollider);
        behaviorAgent.SetVariableValue<Collider>("HurtBox", hurtCollider);

        health = ssoEnemyData.Value.health;
        maxhealth = ssoEnemyData.Value.health;
        navMeshAgent.speed = ssoEnemyData.Value.speedPatrol;
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
        enemyMaxTravelZone.Setup(ssoEnemyData);
    }

    private void OnEnable()
    {
        enemyDetectionRange.onTargetDetected.AddListener(SetTarget);
        enemyHurt.onUpdateEnemyHealth.AddListener(UpdateHealth);
        enemyMaxTravelZone.onTargetDetected.AddListener(SetTarget);
        enemyMaxTravelZone.onTarget.AddListener(SetInsideBox);

        rseOnPlayerDeath.action += PlayerDied;
        rseOnPlayerRespawn.action += PlayerRespawn;
        rseOnDataLoad.action += LoadEnemy;

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
        enemyDetectionRange.onTargetDetected.RemoveListener(SetTarget);
        enemyHurt.onUpdateEnemyHealth.RemoveListener(UpdateHealth);
        enemyMaxTravelZone.onTargetDetected.RemoveListener(SetTarget);
        enemyMaxTravelZone.onTarget.AddListener(SetInsideBox);

        rseOnPlayerDeath.action -= PlayerDied;
        rseOnPlayerRespawn.action -= PlayerRespawn;
        rseOnDataLoad.action -= LoadEnemy;

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

    private void Start()
    {
        S_ClassEnemySaved enemy = new S_ClassEnemySaved
        {
            entity = gameObject,
            isDead = false,
        };

        rsoDataSaved.Value.enemy.Add(enemy);
    }

    private void Update()
    {
        if (isChase && target != null)
        {
            Chase();
        }

        if (isStrafe && target != null)
        {
            Strafing();
        }
    }

    private void FixedUpdate()
    {
        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool(moveParam, isMoving);
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

    private void SetInsideBox(GameObject newTarget)
    {
        targetInZone = newTarget;
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
            newTarget.TryGetComponent<I_AimPointProvider>(out I_AimPointProvider aimPointProvider);
            aimPoint = aimPointProvider != null ? aimPointProvider.GetAimPoint() : newTarget.transform;

            float distance = Vector3.Distance(transform.position, newTarget.transform.position);
            Vector3 dir = (newTarget.transform.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distance, obstacleMask))
            {
                Debug.DrawLine(transform.position, newTarget.transform.position, Color.yellow);

                if (hit.collider.gameObject != newTarget)
                {
                    health = maxhealth;
                    onUpdateEnemyHealth.Invoke(health);

                    behaviorAgent.SetVariableValue<float>("Health", health);

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
            health = maxhealth;
            onUpdateEnemyHealth.Invoke(health);

            behaviorAgent.SetVariableValue<float>("Health", health);

            aimPoint = null;

            detectionCollider.enabled = false;

            behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Patrol);
        }

        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        behaviorAgent.Restart();
    }

    #region Load Enemy
    private void LoadEnemy()
    {
        int index = 0;

        for (int i = 0; i < rsoDataSaved.Value.enemy.Count; i++)
        {
            if (rsoDataSaved.Value.enemy[i].entity == gameObject)
            {
                index = i;
                break;
            }
        }

        if (rsoDataSaved.Value.enemy[index].isDead)
        {
            isDead = true;

            var list = rsoDataSaved.Value.enemy;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].entity == gameObject)
                {
                    list[i].isDead = true;
                    break;
                }
            }

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
    }
    #endregion

    #region Health System
    private void SetHealth(float damage)
    {
        rseOnSendConsoleMessage.Call(gameObject.transform.parent.name + " Take " + damage + " Damage!");

        health = Mathf.Max(health - damage, 0);
        onUpdateEnemyHealth.Invoke(health);

        behaviorAgent.SetVariableValue<float>("Health", health);
        onGetHit.Invoke();

        if (health <= 0)
        {
            isDead = true;

            var list = rsoDataSaved.Value.enemy;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].entity == gameObject)
                {
                    list[i].isDead = true;
                    break;
                }
            }

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

            rseOnSendConsoleMessage.Call(gameObject.transform.parent.name + " is Dead!");
        }
        else if (damage >= maxhealth / 2)
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

            rseOnSendConsoleMessage.Call(gameObject.transform.parent.name + " is Stun!");
        }
    }

    private void UpdateHealth(float damage)
    {
        if (target == null)
        {
            if (targetInZone != null)
            {
                SetTarget(targetInZone);

                SetHealth(damage);

                if (!isDead)
                {
                    behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Chase);
                    behaviorAgent.Restart();
                }
            }
        }
        else
        {
            SetHealth(damage);
        }
    }
    #endregion

    #region Player Death
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

    private void PlayerRespawn()
    {
        isPlayerDead = false;
    }
    #endregion

    #region Patrol System
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

            patrolCoroutine = StartCoroutine(PatrolRoutine());

            rseOnSendConsoleMessage.Call(gameObject.transform.parent.name + " Start Patroling!");
        }
        else
        {
            isPatrolling = false;

            rseOnSendConsoleMessage.Call(gameObject.transform.parent.name + " Stop Patroling!");
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

            navMeshAgent.speed = ssoEnemyData.Value.speedPatrol;
            detectionCollider.enabled = true;

            float waitTime = Random.Range(ssoEnemyData.Value.patrolPointWaitMin, ssoEnemyData.Value.patrolPointWaitMax);

            yield return new WaitForSeconds(waitTime);

            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPointsList.Count;
        }
    }
    #endregion

    #region Chase System
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

            /*navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
            isStrafe = true;*/

            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
            behaviorAgent.SetVariableValue("State", S_EnumEnemyState.Attack);
            behaviorAgent.Restart();
        }
    }

    private void Strafing()
    {
        if (target == null) return;

        if (canAttack)
        {
            isStrafe = false;
            waiting = false;

            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
            behaviorAgent.SetVariableValue("State", S_EnumEnemyState.Attack);
            behaviorAgent.Restart();
        }
        else
        {
            if (waiting)
            {
                if (Time.time < nextWaitEndTime)
                    return; // keep waiting

                waiting = false;   // finished waiting → choose a new direction
                nextChangeTime = Time.time;
            }

            // ---------------------------------------------------------
            // CHANGE DIRECTION PHASE
            // ---------------------------------------------------------
            if (Time.time >= nextChangeTime)
            {
                // Randomly choose left (-1) or right (+1)
                strafeDirection = Random.value > 0.5f ? 1 : -1;

                // How long before next direction change?
                float interval = Random.Range(strafeMinInterval, strafeMaxInterval);
                nextChangeTime = Time.time + interval;
            }

            // ---------------------------------------------------------
            // COMPUTE STRAFE POSITION AROUND PLAYER
            // ---------------------------------------------------------
            Vector3 offsetPlayer = transform.position - target.transform.position;
            offsetPlayer.y = 0;

            if (offsetPlayer.sqrMagnitude < 0.01f)
                offsetPlayer = transform.right;

            // distance circle around player
            Vector3 offsetAtRadius = offsetPlayer.normalized * combo.distanceToChase;

            // perpendicular direction
            Vector3 sideDir = Vector3.Cross(offsetAtRadius, Vector3.up).normalized * strafeDirection;

            // final strafe target
            Vector3 finalPos = target.transform.position + offsetAtRadius + sideDir * strafeOffset;

            navMeshAgent.SetDestination(finalPos);

            Debug.Log(navMeshAgent.remainingDistance);

            // ---------------------------------------------------------
            // CHECK IF ARRIVED → START WAITING
            // ---------------------------------------------------------
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= stoppingDistanceThreshold)
            {
                waiting = true;
                navMeshAgent.velocity = Vector3.zero;
                nextWaitEndTime = Time.time + strafeWaitTime; // pause at the point
            }
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

            rseOnSendConsoleMessage.Call(gameObject.transform.parent.name + " is Chasing the Player!");
        }
        else
        {
            isChase = false;

            rseOnSendConsoleMessage.Call(gameObject.transform.parent.name + " Stop Chasing the Player!");
        }
    }
    #endregion

    #region Combo System
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

    private IEnumerator PlayComboSequence()
    {
        yield return new WaitForSeconds(0.4f);

        rseOnSendConsoleMessage.Call(gameObject.transform.parent.name + " is Attacking with a Combo!");

        isPerformingCombo = true;
        canAttack = false;

        for (int i = 0; i < combo.listAnimationsCombos.Count; i++)
        {
            string overrideKey = (i % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
            overrideController[overrideKey] = combo.listAnimationsCombos[i].animation;

            enemyAttackData.SetAttackMode(combo.listAnimationsCombos[i].attackData);
            animator.SetTrigger(i == 0 ? attackParam : comboParam);

            yield return new WaitForSeconds(combo.listAnimationsCombos[i].animation.length / 2);

            if (combo.listAnimationsCombos[i].attackData.attackType == S_EnumEnemyAttackType.Projectile)
            {
                S_EnemyProjectile projectileInstance = Instantiate(enemyProjectile, spawnProjectilePoint.transform.position, Quaternion.identity);
                projectileInstance.Initialize(bodyCollider.transform, aimPoint, combo.listAnimationsCombos[i].attackData);
            }

            yield return new WaitForSeconds(combo.listAnimationsCombos[i].animation.length / 2);

            /*float distance = Vector3.Distance(body.transform.position, target.transform.position);
            if (distance > combo.distanceToLoseAttack)
            {
                enemyAttackData.DisableWeaponCollider();
                break;
            }*/
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
        yield return new WaitForSeconds(ssoEnemyData.Value.attackCooldown);

        behaviorAgent.SetVariableValue<bool>("CanAttack", true);
        canAttack = true;
    }
    #endregion

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