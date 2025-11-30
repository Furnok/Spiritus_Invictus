using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class S_PlayerInputsManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Player Input")]
    [SerializeField] private PlayerInput playerInput;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerDeath _onPlayerDeathRse;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerRespawn _onPlayerRespawnRse;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUiActionInputEnabled;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameActionInputEnabled;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCinematicInputEnabled rseOnCinematicInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerMove rseOnPlayerMove;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerAttackInput rseOnPlayerAttack;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerAttackInputCancel rseOnPlayerAttackInputCancel;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerDodgeInput rseOnPlayerDodge;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerInteractInput rseOnPlayerInteract;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerParryInput rseOnPlayerParry;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerTargeting rseOnPlayerTargeting;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerTargetingCancel rseOnPlayerTargetingCancel;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerSwapTarget rseOnPlayerSwapTarget;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerHealInput rseOnPlayerHeal;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerDodgeInputCancel rseOnPlayerDodgeInputCancel;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnConsole rseOnConsole;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSkipInput rseOnSkipInput;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSkipCancelInput rseOnSkipCancelInput;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_CurrentInputActionMap rsoCurrentInputActionMap;

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

        rsoCurrentInputActionMap.Value = S_EnumPlayerInputActionMap.None;
    }

    private void OnEnable()
    {
        if (!initialized) return;

        playerInput.actions.Enable();

        rseOnCinematicInputEnabled.action += ActivateCinematicActionInput;
        rseOnGameActionInputEnabled.action += ActivateGameActionInput;
        rseOnUiActionInputEnabled.action += ActivateUIActionInput;

        ActivateGameActionInput();

        _onPlayerDeathRse.action += DeactivateInput;
        _onPlayerRespawnRse.action += ActivateGameActionInput;
    }

    private void OnDisable()
    {
        if (!initialized) return;

        rseOnCinematicInputEnabled.action -= ActivateCinematicActionInput;
        rseOnGameActionInputEnabled.action -= ActivateGameActionInput;
        rseOnUiActionInputEnabled.action -= ActivateUIActionInput;

        _onPlayerDeathRse.action -= DeactivateInput;
        _onPlayerRespawnRse.action -= ActivateGameActionInput;

        playerInput.actions.Disable();
        DisableGameInputs();
        DisableUIInputs();
    }

    private void DeactivateInput()
    {
        if (!initialized) return;

        playerInput.actions.Disable();

        rsoCurrentInputActionMap.Value = S_EnumPlayerInputActionMap.None;
    }

    #region Game Input Callback Methods
    private void OnMoveChanged(InputAction.CallbackContext ctx)
    {
        rseOnPlayerMove.Call(ctx.ReadValue<Vector2>());
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

    private void OnParryInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerParry.Call();
    }

    private void OnHealInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerHeal.Call();
    }

    private void OnConsoleInput(InputAction.CallbackContext ctx)
    {
        rseOnConsole.Call();
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

    #region Enable/Disable Game Inputs
    private void EnableGameInputs()
    {
        var game = iaPlayerInput.Game;

        game.Move.performed += OnMoveChanged;
        game.Move.canceled += OnMoveChanged;
        game.Attack.performed += OnAttackInput;
        game.Attack.canceled += OnAttackInputCancel;
        game.Dodge.performed += OnDodgeInput;
        game.Dodge.canceled += OnDodgeInputCancel;
        game.Interact.performed += OnInteractInput;
        game.Parry.performed += OnParryInput;
        game.Pause.performed += OnPauseGameInput;
        game.Targeting.performed += OnTargetingInput;
        game.Targeting.canceled += OnTargetingCancelInput;
        game.SwapTarget.performed += OnSwapTargetInput;
        game.Heal.performed += OnHealInput;
        game.Console.performed += OnConsoleInput;
    }

    private void DisableGameInputs()
    {
        var game = iaPlayerInput.Game;

        game.Move.performed -= OnMoveChanged;
        game.Move.canceled -= OnMoveChanged;
        game.Attack.performed -= OnAttackInput;
        game.Attack.canceled -= OnAttackInputCancel;
        game.Dodge.performed -= OnDodgeInput;
        game.Dodge.canceled -= OnDodgeInputCancel;
        game.Interact.performed -= OnInteractInput;
        game.Parry.performed -= OnParryInput;
        game.Pause.performed -= OnPauseGameInput;
        game.Targeting.performed -= OnTargetingInput;
        game.Targeting.canceled -= OnTargetingCancelInput;
        game.SwapTarget.performed -= OnSwapTargetInput;
        game.Heal.performed -= OnHealInput;
        game.Console.performed -= OnConsoleInput;
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

        rsoCurrentInputActionMap.Value = S_EnumPlayerInputActionMap.Game;
    }
    #endregion

    #region Enable/Disable UI Inputs
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

    private void ActivateUIActionInput()
    {
        if (!initialized) return;

        DisableGameInputs();
        EnableUIInputs();
        DisableCinematicInputs();

        playerInput.actions.Disable();
        playerInput.SwitchCurrentActionMap(uiMapName);
        playerInput.currentActionMap.Enable();

        rsoCurrentInputActionMap.Value = S_EnumPlayerInputActionMap.UI;
    }
    #endregion

    #region Enable/Disable Cinemactic Inputs
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

    private void ActivateCinematicActionInput()
    {
        if (!initialized) return;

        DisableGameInputs();
        DisableUIInputs();
        EnableCinematicInputs();

        playerInput.actions.Disable();
        playerInput.SwitchCurrentActionMap(cinematicMapName);
        playerInput.currentActionMap.Enable();

        rsoCurrentInputActionMap.Value = S_EnumPlayerInputActionMap.Cinematic;
    }
    #endregion
}