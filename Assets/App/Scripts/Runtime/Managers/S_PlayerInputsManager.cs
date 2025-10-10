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
    [SerializeField] private RSE_OnPlayerAttackInput rseOnPlayerAttack;
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

    private IA_PlayerInput iaPlayerInput = null;
    private bool initialized = false;
    private string gameMapName = "";
    private string uiMapName = "";
    private string cinematicMapName = "";

    private void Awake()
    {
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component missing");
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
        ActivateGameActionInput();
    }

    private void OnEnable()
    {
        if (!initialized) return;

        var game = iaPlayerInput.Game;
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

        rseOnInputDisabled.action += DeactivateInput;
        rseOnCinematicInputEnabled.action += ActivateCinematicActionInput;
        rseOnGameActionInputEnabled.action += ActivateGameActionInput;
        rseOnUiActionInputEnabled.action += ActivateUiActionInput;

        playerInput.actions.Enable();

        playerInput.SwitchCurrentActionMap(gameMapName);
    }

    private void OnDisable()
    {
        if (!initialized) return;

        var game = iaPlayerInput.Game;

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

        rseOnInputDisabled.action -= DeactivateInput;
        rseOnCinematicInputEnabled.action -= ActivateCinematicActionInput;
        rseOnGameActionInputEnabled.action -= ActivateGameActionInput;
        rseOnUiActionInputEnabled.action -= ActivateUiActionInput;

        playerInput.actions.Disable();
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

    private void OnDodgeInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerDodge.Call();
    }

    private void OnInteractInput(InputAction.CallbackContext ctx)
    {
        rseOnPlayerInteract.Call();
    }

    private void OnPauseInput(InputAction.CallbackContext ctx)
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
        playerInput.actions.Enable();
        playerInput.SwitchCurrentActionMap(gameMapName);

        rsoLastInputActionMap.Value = rsoCurrentInputActionMap.Value;
        rsoCurrentInputActionMap.Value = EnumPlayerInputActionMap.Game;
    }

    private void ActivateUiActionInput()
    {
        if (!initialized) return;
        playerInput.actions.Enable();
        playerInput.SwitchCurrentActionMap(uiMapName);

        rsoLastInputActionMap.Value = rsoCurrentInputActionMap.Value;
        rsoCurrentInputActionMap.Value = EnumPlayerInputActionMap.UI;
    }

    private void ActivateCinematicActionInput()
    {
        if (!initialized) return;
        playerInput.actions.Enable();
        playerInput.SwitchCurrentActionMap(cinematicMapName);

        rsoLastInputActionMap.Value = rsoCurrentInputActionMap.Value;
        rsoCurrentInputActionMap.Value = EnumPlayerInputActionMap.Cinematic;
    }

    private void OnGameOver()
    {
        if (!initialized) return;
        playerInput.actions.Enable();
        playerInput.SwitchCurrentActionMap(uiMapName);
    }
}