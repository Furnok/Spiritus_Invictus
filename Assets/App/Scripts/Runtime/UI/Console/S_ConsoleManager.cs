using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_ConsoleManager : MonoBehaviour
{
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
        Message(message);
    }

    public void SendInput()
    {
        string message = inputField.text;
        Message(message);
    }

    private void Message(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        if (string.IsNullOrEmpty(consoleText.text))
            consoleText.text = message;
        else
            consoleText.text = consoleText.text + "\n" + message;

        inputField.text = "";

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}