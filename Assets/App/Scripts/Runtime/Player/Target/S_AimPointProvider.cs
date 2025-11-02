using UnityEngine;

public class S_AimPointProvider : MonoBehaviour, IAimPointProvider
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] GameObject _aimPointObject;
    //[Header("Inputs")]

    //[Header("Outputs")]

    public Transform GetAimPoint()
    {
        return _aimPointObject.transform;
    }
}