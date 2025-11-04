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
    [Title("Scripts")]
    [SerializeField] private S_EnemyAttackData enemyAttackData;

    [TabGroup("References")]
    [SerializeField] private S_EnemyDetectionRange enemyDetectionRange;

    [TabGroup("References")]
    [SerializeField] private S_EnemyHurt enemyHurt;

    [TabGroup("References")]
    [SerializeField] private S_EnemyUI enemyUI;

    [TabGroup("References")]
    [SerializeField] private S_EnemyPatrolPoints enemyPatrolPoints;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerDeath rseOnPlayerDeath;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnEnemyTargetDied rseOnEnemyTargetDied;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnGamePause rseOnGamePause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSpawnEnemyProjectile rseOnSpawnEnemyProjectile;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_EnemyData ssoEnemyData;

    [HideInInspector] public UnityEvent<float> onUpdateEnemyHealth;
    [HideInInspector] public UnityEvent onGetHit;

    private float health = 0;
    private float maxhealth = 0;
    private bool isPaused = false;
    private GameObject target;
    private BlackboardVariable<bool> isAttacking;
    private AnimatorOverrideController overrideController;
    private Coroutine comboCoroutine;
    private Coroutine resetCoroutine;

    private void Awake()
    {
        overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

        behaviorAgent.SetVariableValue<GameObject>("Body", body);
        behaviorAgent.SetVariableValue<List<GameObject>>("PatrolPoints", enemyPatrolPoints.GetPatrolPointsList());
        behaviorAgent.SetVariableValue<Animator>("Animator", animator);
        behaviorAgent.SetVariableValue<Collider>("BodyCollider", bodyCollider);
        behaviorAgent.SetVariableValue<Collider>("DetectionRangeCollider", DetectionCollider);

        health = ssoEnemyData.Value.health;
        maxhealth = ssoEnemyData.Value.health;
        behaviorAgent.SetVariableValue<float>("Health", ssoEnemyData.Value.health);
        behaviorAgent.SetVariableValue<float>("Speed", ssoEnemyData.Value.speed);
        behaviorAgent.SetVariableValue<float>("DistanceToChase", ssoEnemyData.Value.distanceToChase);
        behaviorAgent.SetVariableValue<float>("DistanceToLoseAttack", ssoEnemyData.Value.distanceToLoseAttack);
        behaviorAgent.SetVariableValue<float>("TimeDespawn", ssoEnemyData.Value.timeDespawn);
        behaviorAgent.SetVariableValue<float>("PatrolPointWaitMin", ssoEnemyData.Value.patrolPointWaitMin);
        behaviorAgent.SetVariableValue<float>("PatrolPointWaitMax", ssoEnemyData.Value.patrolPointWaitMax);
        behaviorAgent.SetVariableValue<float>("StartPatrolWaitMin", ssoEnemyData.Value.startPatrolWaitMin);
        behaviorAgent.SetVariableValue<float>("StartPatrolWaitMax", ssoEnemyData.Value.startPatrolWaitMax);
        behaviorAgent.SetVariableValue<float>("TimeBeforeChaseMin", ssoEnemyData.Value.timeBeforeChaseMin);
        behaviorAgent.SetVariableValue<float>("TimeBeforeChaseMax", ssoEnemyData.Value.timeBeforeChaseMax);

        behaviorAgent.SetVariableValue<string>("MoveParam", moveParam);
        behaviorAgent.SetVariableValue<string>("DeathParam", deathParam);
        behaviorAgent.SetVariableValue<string>("AttackIdleParam", idleAttack);
        behaviorAgent.SetVariableValue<string>("AttackParam", attackParam);
        behaviorAgent.SetVariableValue<string>("HitHeavyParam", hitHeavyParam);

        enemyAttackData.Setup(ssoEnemyData);
        enemyDetectionRange.Setup(ssoEnemyData);
        enemyUI.Setup(ssoEnemyData);
    }

    private void OnEnable()
    {
        rseOnGamePause.action += Pause;

        enemyDetectionRange.onTargetDetected.AddListener(SetTarget);
        enemyHurt.onUpdateEnemyHealth.AddListener(UpdateHealth);

        rseOnPlayerDeath.action += EasterEggsTBAG;

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

        rseOnPlayerDeath.action -= EasterEggsTBAG;

        if (isAttacking != null)
        {
            isAttacking.OnValueChanged -= AttackCombo;
        }

        overrideController["AttackAnimation"] = null;
        overrideController["AttackAnimation2"] = null;
    }

    private void Update()
    {
        if (navMeshAgent.velocity.magnitude <= 0.2f)
        {
            animator.SetBool(moveParam, false);
        }
        else
        {
            animator.SetBool(moveParam, true);
        }
    }

    private void Pause(bool value)
    {
        if (value)
        {
            behaviorAgent.enabled = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            animator.speed = 0f;
            isPaused = true;
        }
        else
        {
            behaviorAgent.enabled = true;
            navMeshAgent.isStopped = false;
            animator.speed = 1f;
            isPaused = false;
        }
    }

    private void SetTarget(GameObject newTarget)
    {
        behaviorAgent.SetVariableValue<GameObject>("Target", newTarget);

        target = newTarget;

        if (newTarget != null)
        {
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
            behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Patrol);
        }
    }

    private void UpdateHealth(float damage)
    {
        health = Mathf.Max(health - damage, 0);
        onUpdateEnemyHealth.Invoke(health);

        behaviorAgent.SetVariableValue<float>("Health", health);
        onGetHit.Invoke();

        if (damage >= maxhealth / 2)
        {
            behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.HeavyHit);

            if (comboCoroutine != null)
            {
                StopCoroutine(comboCoroutine);
                comboCoroutine = null;
            }

            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
                resetCoroutine = null;
            }

            resetCoroutine = StartCoroutine(ResetAttack());
        }

        if (health <= 0)
        {
            if (comboCoroutine != null)
            {
                StopCoroutine(comboCoroutine);
                comboCoroutine = null;
            }

            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
                resetCoroutine = null;
            }

            rseOnEnemyTargetDied.Call(body);
            enemyAttackData.DisableWeaponCollider();
        }
    }

    private void EasterEggsTBAG()
    {
        float rnd = Random.Range(0f, 100f);
        float chance = ssoEnemyData.Value.chanceForEasterEgg;

        if (rnd < chance)
        {
            animator.SetTrigger(tBagParam);
        }
        else
        {
            animator.SetBool(idleAttack, false);
            behaviorAgent.SetVariableValue<S_EnumEnemyState>("State", S_EnumEnemyState.Idle);
        }
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
        int rnd = Random.Range(0, ssoEnemyData.Value.listCombos.Count);

        var combo = ssoEnemyData.Value.listCombos[rnd].listAnimationsCombos;

        if (!ssoEnemyData.Value.listCombos[rnd].isProjectile)
        {
            for (int i = 0; i < combo.Count; i++)
            {
                string overrideKey = (i % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
                overrideController[overrideKey] = combo[i];

                animator.SetTrigger(i == 0 ? attackParam : comboParam);

                yield return WaitForSecondsWhileUnpaused(combo[i].length);
            }
        }
        else
        {
            for (int i = 0; i < combo.Count; i++)
            {
                string overrideKey = (i % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
                overrideController[overrideKey] = combo[i];

                animator.SetTrigger(i == 0 ? attackParam : comboParam);

                rseOnSpawnEnemyProjectile.Call(ssoEnemyData.Value.projectileDamage);

                yield return WaitForSecondsWhileUnpaused(combo[i].length);
            }
        }

        isAttacking.Value = false;
        comboCoroutine = null;
        animator.SetTrigger(stopAttackParam);

        resetCoroutine = StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return WaitForSecondsWhileUnpaused(ssoEnemyData.Value.attackCooldown);

        behaviorAgent.SetVariableValue<bool>("CanAttack", true);
    }
}