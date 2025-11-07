using NUnit.Framework;
using UnityEngine;

public class S_ParticleBeam : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] Transform _beamsParent;

    //[Header("Inputs")]

    //[Header("Outputs")]

    private void Update()
    {
        _beamsParent.transform.LookAt(Camera.main.transform);
    }
}