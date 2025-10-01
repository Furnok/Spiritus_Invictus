using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class S_PlayerInputManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerInput _playerInputComponent;
    [SerializeField] RSO_CurrentInputActionMap _currentInputActionMap;
    [SerializeField] RSO_LastInputActionMap _lastInputActionMap;

    [Header("Input")]
    [SerializeField] RSE_OnPlayerMove _onPlayerMove;
    [SerializeField] RSE_OnPlayerAttackInput _onPlayerAttack;
    [SerializeField] RSE_OnPlayerDodgeInput _onPlayerDodge;
    [SerializeField] RSE_OnPlayerInteractInput _onPlayerInteract;
    [SerializeField] RSE_OnPlayerPause _onPlayerPause;
    [SerializeField] RSE_OnPlayerMeditationInput _onPlayerMeditation;
    [SerializeField] RSE_OnPlayerMeditationCancelInput _onPlayerMeditationCancel;
    [SerializeField] RSE_OnPlayerParryInput _onPlayerParry;
    [SerializeField] RSE_OnPlayerTargeting _onPlayerTargeting;
    [SerializeField] RSE_OnPlayerTargetingCancel _onPlayerTargetingCancel;
    [SerializeField] RSE_OnPlayerSwapTarget _onPlayerSwapTarget;
    [SerializeField] RSE_OnPlayerHealInput _OnPlayerHeal;
    [SerializeField] RSE_OnInputDisabled _onInputDisabled;
    [SerializeField] RSE_OnCinematicInputEnabled _onCinematicInputEnabled;
    [SerializeField] RSE_OnGameInputEnabled _onGameActionInputEnabled;
    [SerializeField] RSE_OnUiInputEnabled _onUiActionInputEnabled;

    IA_PlayerInput _playerInput;
    bool _initialized;
    string _gameMapName;
    string _uiMapName;
    string _cinematicMapName;

    private void Awake()
    {
        if (_playerInputComponent == null)
        {
            Debug.LogError("PlayerInput component missing");
            enabled = false;
            return;
        }

        _playerInput = new IA_PlayerInput();
        _playerInputComponent.actions = _playerInput.asset;
        _initialized = true;

        _gameMapName = _playerInput.Game.Get().name;
        _uiMapName = _playerInput.UI.Get().name;
        _cinematicMapName = _playerInput.Cinematic.Get().name;

        _currentInputActionMap.Value = E_PlayerInputActionMap.None;
        _lastInputActionMap.Value = E_PlayerInputActionMap.None;
        ActivateGameActionInput();
    }

    private void OnEnable()
    {
        if (!_initialized) return;

        var game = _playerInput.Game;
        game.Move.performed += OnMoveChanged;
        game.Move.canceled += OnMoveChanged;
        game.Attack.performed += OnAttackInput;
        game.Dodge.performed += OnDodgeInput;
        game.Interact.performed += OnInteractInput;
        game.Meditation.performed += OnMeditationInput;
        game.Meditation.canceled += OnMeditationCancelInput;
        game.Parry.performed += OnParryInput;
        game.Pause.performed += OnPauseInput;
        game.Targeting.performed += OnTargetingInput;
        game.Targeting.canceled += OnTargetingCancelInput;
        game.SwapTarget.performed += OnSwapTargetInput;
        game.Heal.performed += OnHealInput;

        _onInputDisabled.action += DeactivateInput;
        _onCinematicInputEnabled.action += ActivateCinematicActionInput;
        _onGameActionInputEnabled.action += ActivateGameActionInput;
        _onUiActionInputEnabled.action += ActivateUiActionInput;

        _playerInputComponent.actions.Enable();

        _playerInputComponent.SwitchCurrentActionMap(_gameMapName);
    }

    private void OnDisable()
    {
        if (!_initialized) return;

        var game = _playerInput.Game;

        game.Move.performed -= OnMoveChanged;
        game.Move.canceled -= OnMoveChanged;
        game.Attack.performed -= OnAttackInput;
        game.Dodge.performed -= OnDodgeInput;
        game.Interact.performed -= OnInteractInput;
        game.Meditation.performed -= OnMeditationInput;
        game.Meditation.canceled -= OnMeditationCancelInput;
        game.Parry.performed -= OnParryInput;
        game.Pause.performed -= OnPauseInput;
        game.Targeting.performed -= OnTargetingInput;
        game.Targeting.canceled -= OnTargetingCancelInput;
        game.SwapTarget.performed -= OnSwapTargetInput;
        game.Heal.performed -= OnHealInput;

        _onInputDisabled.action -= DeactivateInput;
        _onCinematicInputEnabled.action -= ActivateCinematicActionInput;
        _onGameActionInputEnabled.action -= ActivateGameActionInput;
        _onUiActionInputEnabled.action -= ActivateUiActionInput;

        _playerInputComponent.actions.Disable();
    }

    #region Game Input Callback Methods
    void OnMoveChanged(InputAction.CallbackContext ctx)
    {
        _onPlayerMove.Call(ctx.ReadValue<Vector2>());
    }

    void OnTargetingInput(InputAction.CallbackContext ctx)
    {
        _onPlayerTargeting.Call();
    }

    void OnTargetingCancelInput(InputAction.CallbackContext ctx)
    {
        _onPlayerTargetingCancel.Call();
    }

    void OnSwapTargetInput(InputAction.CallbackContext ctx)
    {
        _onPlayerSwapTarget.Call(ctx.ReadValue<float>());
    }


    void OnAttackInput(InputAction.CallbackContext ctx)
    {
        _onPlayerAttack.Call();
    }

    void OnDodgeInput(InputAction.CallbackContext ctx)
    {
        _onPlayerDodge.Call();
    }

    void OnInteractInput(InputAction.CallbackContext ctx)
    {
        _onPlayerInteract.Call();
    }

    void OnPauseInput(InputAction.CallbackContext ctx)
    {
        _onPlayerPause.Call();
    }

    void OnMeditationInput(InputAction.CallbackContext ctx)
    {
        _onPlayerMeditation.Call();
    }

    void OnMeditationCancelInput(InputAction.CallbackContext ctx)
    {
        _onPlayerMeditationCancel.Call();
    }

    void OnParryInput(InputAction.CallbackContext ctx)
    {
        _onPlayerParry.Call();
    }

    void OnHealInput(InputAction.CallbackContext ctx)
    {
        _OnPlayerHeal.Call();
    }


    #endregion

    private void DeactivateInput()
    {
        if (!_initialized) return;
        _playerInputComponent.actions.Disable();

        _lastInputActionMap.Value = _currentInputActionMap.Value;
        _currentInputActionMap.Value = E_PlayerInputActionMap.None;
    }

    private void ActivateGameActionInput()
    {
        if (!_initialized) return;
        _playerInputComponent.actions.Enable();
        _playerInputComponent.SwitchCurrentActionMap(_gameMapName);

        _lastInputActionMap.Value = _currentInputActionMap.Value;
        _currentInputActionMap.Value = E_PlayerInputActionMap.Game;
    }

    private void ActivateUiActionInput()
    {
        if (!_initialized) return;
        _playerInputComponent.actions.Enable();
        _playerInputComponent.SwitchCurrentActionMap(_uiMapName);

        _lastInputActionMap.Value = _currentInputActionMap.Value;
        _currentInputActionMap.Value = E_PlayerInputActionMap.UI;
    }

    private void ActivateCinematicActionInput()
    {
        if (!_initialized) return;
        _playerInputComponent.actions.Enable();
        _playerInputComponent.SwitchCurrentActionMap(_cinematicMapName);

        _lastInputActionMap.Value = _currentInputActionMap.Value;
        _currentInputActionMap.Value = E_PlayerInputActionMap.Cinematic;
    }

    private void OnGameOver()
    {
        if (!_initialized) return;
        _playerInputComponent.actions.Enable();
        _playerInputComponent.SwitchCurrentActionMap(_uiMapName);
    }
}

public enum E_PlayerInputActionMap
{
    Game,
    UI,
    Cinematic,
    None
}