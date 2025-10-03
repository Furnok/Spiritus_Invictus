using UnityEngine;

public class S_PlayerPause : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    private void OnEnable()
    {
        rseOnPlayerPause.action += Pause;
    }

    private void OnDisable()
    {
        rseOnPlayerPause.action -= Pause;
    }

    private void Pause()
    {
        
    }
}