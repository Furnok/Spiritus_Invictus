using Sirenix.OdinInspector;
using UnityEngine;

public class S_LookAt : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Aim Point")]
    [SerializeField] private GameObject _aimPointObject;

    public Vector3 GetAimPoint()
    {
        return _aimPointObject.transform.position;
    }
}