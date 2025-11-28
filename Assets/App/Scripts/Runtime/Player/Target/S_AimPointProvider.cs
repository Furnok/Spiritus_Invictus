using Sirenix.OdinInspector;
using UnityEngine;

public class S_AimPointProvider : MonoBehaviour, I_AimPointProvider
{
    [TabGroup("References")]
    [Title("Aim Point")]
    [SerializeField] private GameObject _aimPointObject;

    public Transform GetAimPoint()
    {
        return _aimPointObject.transform;
    }
}