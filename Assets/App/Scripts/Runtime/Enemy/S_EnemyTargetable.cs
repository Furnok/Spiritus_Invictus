using UnityEngine;

public class S_EnemyTargetable : MonoBehaviour, ITargetable
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] Transform _lockOnAnchor;

    //[Header("Inputs")]

    //[Header("Outputs")]

    public Transform GetTargetLockOnAnchorTransform()
    {
        return _lockOnAnchor;
    }
}