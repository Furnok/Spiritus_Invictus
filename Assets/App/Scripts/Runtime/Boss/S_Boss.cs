using Sirenix.OdinInspector;
using Unity.Behavior;
using UnityEngine;

public class S_Boss : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Animations Parameters")]
    [SerializeField, S_AnimationName] private string moveParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string deathParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string attackParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string stunParam;

    [TabGroup("References")]
    [Title("Agent")]
    [SerializeField] private BehaviorGraphAgent behaviorAgent;

    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider bodyCollider;

    [TabGroup("References")]
    [SerializeField] private Collider detectionCollider;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [Title("Scripts")]
    [SerializeField] private S_BossDetectionRange bossDetectionRange;

    private GameObject target = null;
    private bool isDead = false;
    private void Awake()
    {
        behaviorAgent.SetVariableValue<Animator>("Animator", animator);
        behaviorAgent.SetVariableValue<float>("Health", 100);
        behaviorAgent.SetVariableValue<float>("MoveSpeed", 5);
        behaviorAgent.SetVariableValue<Collider>("BodyCollider", bodyCollider);
        behaviorAgent.SetVariableValue<string>("DeathParam", deathParam);
        behaviorAgent.SetVariableValue<string>("MoveParam", moveParam);
        behaviorAgent.SetVariableValue<string>("StunParam", stunParam);
        behaviorAgent.SetVariableValue<string>("AttackParam", attackParam);
    }
    private void OnEnable()
    {
        bossDetectionRange.onTargetDetected.AddListener(SetTarget);
    }
    private void OnDisable()
    {
        bossDetectionRange.onTargetDetected.RemoveListener(SetTarget);
    }
    void SetTarget(GameObject newTarget)
    {
        behaviorAgent.SetVariableValue<GameObject>("Target", newTarget);
        behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Chase);
    }
}