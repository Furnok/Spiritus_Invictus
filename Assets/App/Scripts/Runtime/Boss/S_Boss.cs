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
    //[Header("Inputs")]

    //[Header("Outputs")]
}