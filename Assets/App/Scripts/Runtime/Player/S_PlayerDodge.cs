using UnityEngine;

public class S_PlayerDodge : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPlayerDodgeInput rseOnPlayerDodge;

    private void OnEnable()
    {
        rseOnPlayerDodge.action += TryDodge;
    }

    private void OnDisable()
    {
        rseOnPlayerDodge.action -= TryDodge;
    }

    private void TryDodge()
    {

    }
}