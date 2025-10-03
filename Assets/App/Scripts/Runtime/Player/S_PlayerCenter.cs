using UnityEngine;

public class S_PlayerCenter : MonoBehaviour
{
    [Header("Output")]
    [SerializeField] private RSE_OnPlayerCenter rseOnPlayerCenter;

    private void Start()
    {
        rseOnPlayerCenter.Call(transform);
    }
}