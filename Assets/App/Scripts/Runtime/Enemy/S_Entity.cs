using UnityEngine;
using Unity.Behavior;
using UnityEngine.AI;

public class S_Entity : MonoBehaviour
{
    [Header("Settings")]
    [S_AnimationName][SerializeField] string moveParam;
    [S_AnimationName][SerializeField] string attackParam;
    [S_AnimationName][SerializeField] string hitParam;
    [S_AnimationName][SerializeField] string deathParam;
    [S_AnimationName][SerializeField] string combo1Param;
    [S_AnimationName][SerializeField] string combo2Param;

    [Header("References")]
    [SerializeField] BehaviorGraphAgent agent;
    [SerializeField] NavMeshAgent enemyNavMesh;
    [SerializeField] S_EnemyRangeDetection S_EnemyRangeDetection;
    [SerializeField] S_EnemyHealth S_EnemyHealth;
    [SerializeField] S_EnemyAnimation S_EnemyAnimation;

    private void OnEnable()
    {
        S_EnemyRangeDetection.onTargetDetected.AddListener(SetTarget);
        S_EnemyHealth.onUpdateEnemyHealth.AddListener(UpdateHealth);
        S_EnemyHealth.onInitializeEnemyHealth.AddListener(SetHealth);
        S_EnemyAnimation.UpdateTimerAnimation.AddListener(UpdateTimerAnimation);
    }
    private void OnDisable()
    {
        S_EnemyRangeDetection.onTargetDetected.RemoveListener(SetTarget);
        S_EnemyHealth.onUpdateEnemyHealth.RemoveListener(UpdateHealth);
        S_EnemyHealth.onInitializeEnemyHealth.RemoveListener(SetHealth);
        S_EnemyAnimation.UpdateTimerAnimation.RemoveListener(UpdateTimerAnimation);
    }
    private void Awake()
    {
        agent.SetVariableValue<string>("MoveParam", moveParam);
        agent.SetVariableValue<string>("AttackParam", attackParam);
        agent.SetVariableValue<string>("HitParam", hitParam);
        agent.SetVariableValue<string>("DeathParam", deathParam);
        agent.SetVariableValue<string>("Combo1Param", combo1Param);
        agent.SetVariableValue<string>("Combo2Param", combo2Param);
    }
    private void Update()
    {
        agent.SetVariableValue<float>("StopDistance", enemyNavMesh.stoppingDistance);
    }

    void SetTarget(GameObject Target)
    {
        agent.SetVariableValue<GameObject>("Player", Target);
        if(Target != null)
        {
            agent.SetVariableValue<S_EnumEnemyState>("EnemyState", S_EnumEnemyState.Chase);
        }
        else
        {
            agent.SetVariableValue<S_EnumEnemyState>("EnemyState", S_EnumEnemyState.Patrol);
        }
    }
    void SetHealth(float health)
    {
        agent.SetVariableValue<float>("Health", health);
    }
    void UpdateHealth(float health)
    {
        agent.SetVariableValue<float>("Health", health);
        agent.SetVariableValue<S_EnumEnemyState>("EnemyState", S_EnumEnemyState.Hit);
    }
    void UpdateTimerAnimation(float timer)
    {
        agent.SetVariableValue<float>("AnimationTimer", timer);
    }
}
