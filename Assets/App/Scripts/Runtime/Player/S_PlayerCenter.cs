using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerCenter : MonoBehaviour
{
    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerCenter rseOnPlayerCenter;

    private void Start()
    {
        rseOnPlayerCenter.Call(transform);
    }
}