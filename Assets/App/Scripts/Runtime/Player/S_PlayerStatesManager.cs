using UnityEngine;

public class S_PlayerStatesManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;

    [Header("Input")]
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;
    [SerializeField] RSE_OnPlayerRemoveState _onPlayerRemoveState;

    //[Header("Output")]

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

        _playerCurrentState.Value = PlayerState.None;
    }
    void AddState(PlayerState state)
    {
        if (state == _playerCurrentState.Value) return;
        _playerCurrentState.Value = state;
    }

    void RemoveState(PlayerState state)
    {
        _playerCurrentState.Value -= state;
    }
}