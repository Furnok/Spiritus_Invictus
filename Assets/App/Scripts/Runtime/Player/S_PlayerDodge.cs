using UnityEngine;

public class S_PlayerDodge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SSO_PlayerStateTransitions _ssoPlayerStateTransitions;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerDodgeInput rseOnPlayerDodge;
    [SerializeField] private RSE_OnPlayerMove _rseOnPlayerMove;

    Vector2 _moveInput;

    private void OnEnable()
    {
        rseOnPlayerDodge.action += TryDodge;
        _rseOnPlayerMove.action += Move;
    }

    private void OnDisable()
    {
        rseOnPlayerDodge.action -= TryDodge;
        _rseOnPlayerMove.action -= Move;
    }

    private void TryDodge()
    {
        // Check if can dodge
        if (true)
        {
            if(_moveInput != Vector2.zero)
            {
                // Dodge in direction of the movement input
            }
            else
            {
                // Backflip
            }

        }
    }

    private void Move(Vector2 input)
    {
        _moveInput = input;
    }
}