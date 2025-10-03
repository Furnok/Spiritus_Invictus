using UnityEngine;
using Unity.Behavior;
using UnityEngine.AI;

public class S_Entity : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [S_AnimationName] private string moveParam;
    [SerializeField] [S_AnimationName] private string attackParam;
    [SerializeField] [S_AnimationName] private string hitParam;
    [SerializeField] [S_AnimationName] private string deathParam;
    [SerializeField] [S_AnimationName] private string combo1Param;
    [SerializeField] [S_AnimationName] private string combo2Param;

    [Header("References")]
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private NavMeshAgent enemyNavMesh;
    [SerializeField] private S_EnemyRangeDetection S_EnemyRangeDetection;
    [SerializeField] private S_EnemyHealth S_EnemyHealth;
    [SerializeField] private S_EnemyAnimation S_EnemyAnimation;
    [SerializeField] private Animator animator;

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
        //S_EnemyAnimation.UpdateTimerAnimation.AddListener(UpdateTimerAnimation);
    }

    private void OnDisable()
    {
        S_EnemyRangeDetection.onTargetDetected.RemoveListener(SetTarget);
        S_EnemyHealth.onUpdateEnemyHealth.RemoveListener(UpdateHealth);
        S_EnemyHealth.onInitializeEnemyHealth.RemoveListener(SetHealth);
        //S_EnemyAnimation.UpdateTimerAnimation.RemoveListener(UpdateTimerAnimation);
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
            agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.Chase);
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
        agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.Hit);
    }

    private void UpdateTimerAnimation(float timer)
    {
        agent.SetVariableValue<float>("AnimationTimer", timer);
    }
}
