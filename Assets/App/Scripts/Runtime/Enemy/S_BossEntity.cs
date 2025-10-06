using Unity.Behavior;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class S_BossEntity : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField][S_AnimationName] private string moveParam;
    [SerializeField][S_AnimationName] private string attackParam;
    [SerializeField][S_AnimationName] private string hitParam;
    [SerializeField][S_AnimationName] private string deathParam;

    [Header("References")]
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private NavMeshAgent enemyNavMesh;
    [SerializeField] private S_EnemyRangeDetection S_EnemyRangeDetection;
    [SerializeField] private S_EnemyHealth S_EnemyHealth;
    [SerializeField] private Animator animator;
    //[Header("Input")]

    //[Header("Output")]
    private void Awake()
    {
        agent.SetVariableValue<string>("MoveParam", moveParam);
        agent.SetVariableValue<string>("AttackParam", attackParam);
        agent.SetVariableValue<string>("HitParam", hitParam);
        agent.SetVariableValue<string>("DeathParam", deathParam);
    }
    private void OnEnable()
    {
        S_EnemyRangeDetection.onTargetDetected.AddListener(SetTarget);
    }

    private void OnDisable()
    {
        S_EnemyRangeDetection.onTargetDetected.RemoveListener(SetTarget);
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

        if (Target != null)
        {
            agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.Chase);
        }
        else
        {
            agent.SetVariableValue<S_EnumEnemyState>("S_EnumEnemyState", S_EnumEnemyState.Idle);
        }
    }
}