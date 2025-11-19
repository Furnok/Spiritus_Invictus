using FMODUnity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_ConsoleManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Audio")]
    [SerializeField] private EventReference uiSound;

    [TabGroup("References")]
    [Title("InputField")]
    [SerializeField] private TMP_InputField inputField;

    [TabGroup("References")]
    [Title("Console")]
    [SerializeField] private TextMeshProUGUI consoleText;

    [TabGroup("References")]
    [SerializeField] private ScrollRect scrollRect;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnSendConsoleMessage rseOnSendConsoleMessage;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnResetFocus rseOnResetFocus;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    private bool isInputField = false;

    private void OnEnable()
    {
        rseOnSendConsoleMessage.action += SendMessageConsole;
    }

    private void OnDisable()
    {
        rseOnSendConsoleMessage.action -= SendMessageConsole;
    }

    private void SendMessageConsole(string message)
    {
        UpdateUI(message, false);
    }

    public void InputFieldSendInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            rseOnResetFocus.Call();
            rsoNavigation.Value.selectableFocus = null;

            string message = inputField.text;
            if (string.IsNullOrWhiteSpace(message))
            {
                inputField.text = "";
                inputField.Select();
                inputField.caretPosition = 0;
                return;
            }

            RuntimeManager.PlayOneShot(uiSound);

            isInputField = true;
            Message(message);
        }
    }

    public void ButtonSendInput()
    {
        if (Gamepad.current == null)
        {
            rseOnResetFocus.Call();
            rsoNavigation.Value.selectableFocus = null;
        }

        string message = inputField.text;
        if (string.IsNullOrWhiteSpace(message))
        {
            inputField.text = "";
            inputField.caretPosition = 0;
            return;
        }

        isInputField = false;
        Message(message);
    }

    private void Message(string message)
    {
        UpdateUI(message, true);

        StartCoroutine(S_Utils.Delay(0.1f, () =>
        {
            if (isInputField)
            {
                inputField.Select();
                inputField.caretPosition = 0;
            }

            isInputField = false;
        }));
    }

    private void UpdateUI(string message, bool resetInputField)
    {
        if (string.IsNullOrEmpty(consoleText.text))
            consoleText.text = message;
        else
            consoleText.text = consoleText.text + "\n" + message;

        if (resetInputField)
        {
            inputField.text = "";
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}