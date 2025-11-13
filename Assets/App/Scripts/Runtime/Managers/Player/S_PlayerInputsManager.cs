using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class S_PlayerInputsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private RSO_CurrentInputActionMap rsoCurrentInputActionMap;
    [SerializeField] private RSO_LastInputActionMap rsoLastInputActionMap;

    [Header("Output")]
    [SerializeField] private RSE_OnPlayerMove rseOnPlayerMove;
    [SerializeField] private RSE_OnPlayerMoveInputCancel _onPlayerMoveInputCancel;
    [SerializeField] private RSE_OnPlayerAttackInput rseOnPlayerAttack;
    [SerializeField] private RSE_OnPlayerAttackInputCancel rseOnPlayerAttackInputCancel;
    [SerializeField] private RSE_OnPlayerDodgeInput rseOnPlayerDodge;
    [SerializeField] private RSE_OnPlayerInteractInput rseOnPlayerInteract;
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;
    [SerializeField] private RSE_OnPlayerMeditationInput rseOnPlayerMeditation;
    [SerializeField] private RSE_OnPlayerMeditationCancelInput rseOnPlayerMeditationCancel;
    [SerializeField] private RSE_OnPlayerParryInput rseOnPlayerParry;
    [SerializeField] private RSE_OnPlayerTargeting rseOnPlayerTargeting;
    [SerializeField] private RSE_OnPlayerTargetingCancel rseOnPlayerTargetingCancel;
    [SerializeField] private RSE_OnPlayerSwapTarget rseOnPlayerSwapTarget;
    [SerializeField] private RSE_OnPlayerHealInput rseOnPlayerHeal;
    [SerializeField] private RSE_OnInputDisabled rseOnInputDisabled;
    [SerializeField] private RSE_OnCinematicInputEnabled rseOnCinematicInputEnabled;
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameActionInputEnabled;
    [SerializeField] private RSE_OnUIInputEnabled rseOnUiActionInputEnabled;
    [SerializeField] private RSE_OnPlayerDodgeInputCancel rseOnPlayerDodgeInputCancel;
    [SerializeField] private RSE_OnConsole rseOnConsole;
    [SerializeField] private RSE_OnSkipInput rseOnSkipInput;
    [SerializeField] private RSE_OnSkipCancelInput rseOnSkipCancelInput;

    private IA_PlayerInput iaPlayerInput = null;
    private bool initialized = false;
    private string gameMapName = "";
    private string uiMapName = "";
    private string cinematicMapName = "";

    private void Awake()
    {
        if (playerInput == null)
        {
            enabled = false;
            return;
        }

        iaPlayerInput = new IA_PlayerInput();
        playerInput.actions = iaPlayerInput.asset;

        initialized = true;

        gameMapName = iaPlayerInput.Game.Get().name;
        uiMapName = iaPlayerInput.UI.Get().name;
        cinematicMapName = iaPlayerInput.Cinematic.Get().name;

        rsoCurrentInputActionMap.Value = EnumPlayerInputActionMap.None;
        rsoLastInputActionMap.Value = EnumPlayerInputActionMap.None;
    }

    private void OnEnable()
    {
        if (!initialized) return;

        playerInput.actions.Enable();

        rseOnInputDisabled.action += DeactivateInput;
        rseOnCinematicInputEnabled.action += ActivateCinematicActionInput;
        rseOnGameActionInputEnabled.action += ActivateGameActionInput;
        rseOnUiActionInputEnabled.action += ActivateUIActionInput;

        ActivateGameActionInput();
    }

    private void OnDisable()
    {
        if (!initialized) return;

        rseOnInputDisabled.action -= DeactivateInput;
        rseOnCinematicInputEnabled.action -= ActivateCinematicActionInput;
        rseOnGameActionInputEnabled.action -= ActivateGameActionInput;
        rseOnUiActionInputEnabled.action -= ActivateUIActionInput;

        playerInput.actions.Disable();
        DisableGameInputs();
        DisableUIInputs();
    }

    #region Game Input Callback Methods
    private void OnMoveChanged(InputAction.CallbackContext ctx)
    {
        rseOnPlayerMove.Call(ctx.ReadValue<Vector2>());
    }

    private void OnMoveInputCancel(InputAction.CallbackContext ctx)
    {
        _onPlayerMoveInputCancel.Call();
    }

    private void OnTargetingInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerTargeting.Call();
    }

    private void OnTargetingCancelInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerTargetingCancel.Call();
    }

    private void OnSwapTargetInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerSwapTarget.Call(ctx.ReadValue<float>());
    }

    private void OnAttackInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerAttack.Call();
    }

    private void OnAttackInputCancel(InputAction.CallbackContext ctx)
    {
        rseOnPlayerAttackInputCancel.Call();
    }

    private void OnDodgeInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerDodge.Call();
    }

    private void OnDodgeInputCancel(InputAction.CallbackContext ctx)
    {
        rseOnPlayerDodgeInputCancel.Call();
    }

    private void OnInteractInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerInteract.Call();
    }

    private void OnPauseGameInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerPause.Call();
    }

    private void OnMeditationInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerMeditation.Call();
    }

    private void OnMeditationCancelInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerMeditationCancel.Call();
    }

    private void OnParryInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerParry.Call();
    }

    private void OnHealInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerHeal.Call();
    }
    #endregion

    #region UI Input Callback Methods
    private void OnPauseUIInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerPause.Call();
    }

    private void OnConsoleUIInput(InputAction.CallbackContext ctx)
    {
        rseOnConsole.Call();
    }
    #endregion

    #region Cinematic Input Callback Methods

    private void OnSkipInput(InputAction.CallbackContext ctx)
    {
        rseOnSkipInput.Call();
    }

    private void OnSkipCancelInput(InputAction.CallbackContext ctx)
    {
        rseOnSkipCancelInput.Call();
    }
    #endregion

    private void EnableGameInputs()
    {
        var game = iaPlayerInput.Game;

        game.Move.performed += OnMoveChanged;
        game.Move.canceled += OnMoveChanged;
        game.Move.canceled += OnMoveInputCancel;
        game.Attack.performed += OnAttackInput;
        game.Attack.canceled += OnAttackInputCancel;
        game.Dodge.performed += OnDodgeInput;
        game.Dodge.canceled += OnDodgeInputCancel;
        game.Interact.performed += OnInteractInput;
        game.Meditation.performed += OnMeditationInput;
        game.Meditation.canceled += OnMeditationCancelInput;
        game.Parry.performed += OnParryInput;
        game.Pause.performed += OnPauseGameInput;
        game.Targeting.performed += OnTargetingInput;
        game.Targeting.canceled += OnTargetingCancelInput;
        game.SwapTarget.performed += OnSwapTargetInput;
        game.Heal.performed += OnHealInput;
    }

    private void DisableGameInputs()
    {
        var game = iaPlayerInput.Game;

        game.Move.performed -= OnMoveChanged;
        game.Move.canceled -= OnMoveChanged;
        game.Move.canceled -= OnMoveInputCancel;
        game.Attack.performed -= OnAttackInput;
        game.Attack.canceled -= OnAttackInputCancel;
        game.Dodge.performed -= OnDodgeInput;
        game.Dodge.canceled -= OnDodgeInputCancel;
        game.Interact.performed -= OnInteractInput;
        game.Meditation.performed -= OnMeditationInput;
        game.Meditation.canceled -= OnMeditationCancelInput;
        game.Parry.performed -= OnParryInput;
        game.Pause.performed -= OnPauseGameInput;
        game.Targeting.performed -= OnTargetingInput;
        game.Targeting.canceled -= OnTargetingCancelInput;
        game.SwapTarget.performed -= OnSwapTargetInput;
        game.Heal.performed -= OnHealInput;
    }

    private void EnableUIInputs()
    {
        var ui = iaPlayerInput.UI;

        ui.Pause.performed += OnPauseUIInput;
        ui.Console.performed += OnConsoleUIInput;
    }

    private void DisableUIInputs()
    {
        var ui = iaPlayerInput.UI;

        ui.Pause.performed -= OnPauseUIInput;
        ui.Console.performed -= OnConsoleUIInput;
    }

    private void EnableCinematicInputs()
    {
        var cinematic = iaPlayerInput.Cinematic;

        cinematic.Skip.performed += OnSkipInput;
        cinematic.Skip.canceled += OnSkipCancelInput;
    }

    private void DisableCinematicInputs()
    {
        var cinematic = iaPlayerInput.Cinematic;

        cinematic.Skip.performed -= OnSkipInput;
        cinematic.Skip.canceled -= OnSkipCancelInput;
    }

    private void DeactivateInput()
    {
        if (!initialized) return;

        playerInput.actions.Disable();

        rsoLastInputActionMap.Value = rsoCurrentInputActionMap.Value;
        rsoCurrentInputActionMap.Value = EnumPlayerInputActionMap.None;
    }

    private void ActivateGameActionInput()
    {
        if (!initialized) return;

        EnableGameInputs();
        DisableUIInputs();
        DisableCinematicInputs();

        playerInput.actions.Disable();
        playerInput.SwitchCurrentActionMap(gameMapName);
        playerInput.currentActionMap.Enable();

        rsoLastInputActionMap.Value = rsoCurrentInputActionMap.Value;
        rsoCurrentInputActionMap.Value = EnumPlayerInputActionMap.Game;
    }

    private void ActivateUIActionInput()
    {
        if (!initialized) return;

        DisableGameInputs();
        EnableUIInputs();
        DisableCinematicInputs();

        playerInput.actions.Disable();
        playerInput.SwitchCurrentActionMap(uiMapName);
        playerInput.currentActionMap.Enable();

        rsoLastInputActionMap.Value = rsoCurrentInputActionMap.Value;
        rsoCurrentInputActionMap.Value = EnumPlayerInputActionMap.UI;
    }

    private void ActivateCinematicActionInput()
    {
        if (!initialized) return;

        DisableGameInputs();
        DisableUIInputs();
        EnableCinematicInputs();

        playerInput.actions.Disable();
        playerInput.SwitchCurrentActionMap(cinematicMapName);
        playerInput.currentActionMap.Enable();

        rsoLastInputActionMap.Value = rsoCurrentInputActionMap.Value;
        rsoCurrentInputActionMap.Value = EnumPlayerInputActionMap.Cinematic;
    }
}