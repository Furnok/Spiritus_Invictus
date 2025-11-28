using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerStatesManager : MonoBehaviour
{
    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerAddState _onPlayerAddState;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerRemoveState _onPlayerRemoveState;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerCurrentState _playerCurrentState;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStateTransitions _playerStateTransitions;

    private void OnEnable()
    {
        _playerCurrentState.Value = PlayerState.None;

        _onPlayerAddState.action += AddState;
        _onPlayerRemoveState.action += RemoveState;
    }

    private void OnDisable()
    {
        _onPlayerAddState.action -= AddState;
        _onPlayerRemoveState.action -= RemoveState;
    }

    private void AddState(PlayerState state)
    {
        if (state == _playerCurrentState.Value) return;

        _playerCurrentState.Value = state;
    }

    private void RemoveState(PlayerState state)
    {
        _playerCurrentState.Value -= state;
    }
}