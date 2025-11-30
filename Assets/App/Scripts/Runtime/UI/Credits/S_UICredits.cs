using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_UICredits : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Audio")]
    [SerializeField] private EventReference uiSound;

    [TabGroup("References")]
    [Title("Default")]
    [SerializeField] private GameObject defaultWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCloseWindow rseOnCloseWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnShowMouseCursor rseOnShowMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_CurrentWindows rsoCurrentWindows;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InConsole rsoInConsole;

    private bool isClosing = false;

    private void OnEnable()
    {
        rseOnPlayerPause.action += CloseEscape;

        if (Gamepad.current != null)
        {
            rseOnShowMouseCursor.Call();
        }

        isClosing = false;
    }

    private void OnDisable()
    {
        rseOnPlayerPause.action -= CloseEscape;

        isClosing = false;
    }

    private void CloseEscape()
    {
        if (!isClosing)
        {
            if (rsoCurrentWindows.Value[^1] == gameObject && !rsoInConsole.Value)
            {
                Close();
            }
        }
    }

    public void Close()
    {
        if (!isClosing)
        {
            isClosing = true;
            rseOnCloseWindow.Call(gameObject);

            RuntimeManager.PlayOneShot(uiSound);

            if (rsoNavigation.Value.selectablePressOldWindow == null)
            {
                rsoNavigation.Value.selectableFocus = null;
                defaultWindow.SetActive(true);
            }
            else
            {
                rsoNavigation.Value.selectablePressOldWindow?.Select();
                rsoNavigation.Value.selectablePressOldWindow = null;
            }
        }
    }
}