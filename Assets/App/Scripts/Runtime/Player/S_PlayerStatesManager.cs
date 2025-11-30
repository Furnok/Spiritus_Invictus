using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerStatesManager : MonoBehaviour
{
    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerAddState _onPlayerAddState;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerCurrentState _playerCurrentState;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStateTransitions _playerStateTransitions;

    private void OnEnable()
    {
        _playerCurrentState.Value = S_EnumPlayerState.None;

        _onPlayerAddState.action += AddState;
    }

    private void OnDisable()
    {
        _onPlayerAddState.action -= AddState;
    }

    private void AddState(S_EnumPlayerState state)
    {
        if (state == _playerCurrentState.Value) return;

        _playerCurrentState.Value = state;
    }
}