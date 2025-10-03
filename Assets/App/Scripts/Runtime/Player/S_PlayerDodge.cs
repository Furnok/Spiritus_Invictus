using UnityEngine;

public class S_PlayerDodge : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPlayerDodgeInput rseOnPlayerDodge;

    private void OnEnable()
    {
        rseOnPlayerDodge.action += Dodge;
    }

    private void OnDisable()
    {
        rseOnPlayerDodge.action -= Dodge;
    }

    private void Dodge()
    {

    }
}