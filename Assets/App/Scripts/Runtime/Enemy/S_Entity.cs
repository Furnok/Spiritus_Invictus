using UnityEngine;
using Unity.Behavior;
using UnityEngine.AI;
using UnityEngine.Events;

public class S_Entity : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, S_AnimationName] private string moveParam;
    [SerializeField, S_AnimationName] private string attackParam;
    [SerializeField, S_AnimationName] private string hitParam;
    [SerializeField, S_AnimationName] private string deathParam;
    [SerializeField, S_AnimationName] private string combo1Param;
    [SerializeField, S_AnimationName] private string combo2Param;
    [SerializeField, S_AnimationName] private string crouchParam;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float chanceForEasterEgg;

    [Header("References")]
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private NavMeshAgent enemyNavMesh;
    [SerializeField] private S_EnemyRangeDetection S_EnemyRangeDetection;
    [SerializeField] private S_EnemyHealth S_EnemyHealth;
    [SerializeField] private Animator animator;

    [HideInInspector] public UnityEvent onGetHit;

    [Header("InpUt")]
    [SerializeField] RSE_OnPlayerDeath RSE_OnPLayerDeath;

    [Header("References")]
    [SerializeField] private SSO_EnemyHealth ssoEnemyHealthMax;

    private void Awake()
    {
        agent.SetVariableValue<string>("MoveParam", moveParam);
        agent.SetVariableValue<string>("AttackParam", attackParam);
        agent.SetVariableValue<string>("HitParam", hitParam);
        agent.SetVariableValue<string>("DeathParam", deathParam);
        agent.SetVariableValue<string>("Combo1Param", combo1Param);
        agent.SetVariableValue<string>("Combo2Param", combo2Param);
    }

    private void OnEnable()
    {
        S_EnemyRangeDetection.onTargetDetected.AddListener(SetTarget);
        S_EnemyHealth.onUpdateEnemyHealth.AddListener(UpdateHealth);
        S_EnemyHealth.onInitializeEnemyHealth.AddListener(SetHealth);
        RSE_OnPLayerDeath.action += PlayAnimationEasterEgg;
    }

    private void OnDisable()
    {
        S_EnemyRangeDetection.onTargetDetected.RemoveListener(SetTarget);
        S_EnemyHealth.onUpdateEnemyHealth.RemoveListener(UpdateHealth);
        S_EnemyHealth.onInitializeEnemyHealth.RemoveListener(SetHealth);
        RSE_OnPLayerDeath.action -= PlayAnimationEasterEgg;
    }

    private void Update()
    {
        agent.SetVariableValue<float>("StopDistance", enemyNavMesh.stoppingDistance);

        if (enemyNavMesh.velocity.magnitude <= 0.2f)
        {
            animator.SetBool(moveParam, false);
        }
        else
        {
            animator.SetBool(moveParam, true);
        }
    }

    private void SetTarget(GameObject Target)
    {
        agent.SetVariableValue<GameObject>("Player", Target);

        if(Target != null)
        {
            float distance = Vector3.Distance(transform.position, Target.transform.position);
            Vector3 dir = (Target.transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distance, obstacleMask))
            {
                Debug.DrawLine(transform.position, Target.transform.position, Color.yellow);
                if (hit.collider.gameObject != Target)
                {
                    agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.Patrol);
                }
                else
                {
                    agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.Chase);
                }
            }
            else
            {
                agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.Chase);
            }
        }
        else
        {
            agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.Patrol);
        }
    }

    private void SetHealth(float health)
    {
        agent.SetVariableValue<float>("Health", health);
    }

    private void UpdateHealth(float health)
    {
        agent.SetVariableValue<float>("Health", health);
        onGetHit.Invoke();
        agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.LightHit);
    }

    private void PlayAnimationEasterEgg()
    {
        float rnd = Random.Range(0, chanceForEasterEgg);
        if(rnd == 1)
        {
            animator.SetTrigger(crouchParam);
        }
        else
        {
            agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.Idle);
        }
    }
}
