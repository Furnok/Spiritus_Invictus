using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyTargetable : MonoBehaviour, I_Targetable
{
    [TabGroup("References")]
    [SerializeField] private Transform _lockOnAnchor;

    public Transform GetTargetLockOnAnchorTransform()
    {
        return _lockOnAnchor;
    }
}