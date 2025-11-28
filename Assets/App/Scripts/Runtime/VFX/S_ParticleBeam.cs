using Sirenix.OdinInspector;
using UnityEngine;

public class S_ParticleBeam : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Parent")]
    [SerializeField] private Transform _beamsParent;

    private void Update()
    {
        _beamsParent.transform.LookAt(Camera.main.transform);
    }
}