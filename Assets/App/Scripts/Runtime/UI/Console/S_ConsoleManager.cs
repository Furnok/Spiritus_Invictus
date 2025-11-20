using FMODUnity;
using Sirenix.OdinInspector;
using System.Linq;
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

    [TabGroup("Outputs")]
    [SerializeField] private RSO_ConsoleCheats rsoConsoleCheats;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_ConsoleHelper ssoConsoleHelper;

    private bool isInputField = false;

    private void Awake()
    {
        rsoConsoleCheats.Value = new();
    }

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
        CheckCommand(message);

        StartCoroutine(S_Utils.Delay(0.1f, () =>
        {
            if (isInputField)
            {
                inputField.ActivateInputField();
                inputField.Select();
                inputField.caretPosition = 0;
            }

            isInputField = false;
        }));
    }

    private void CheckCommand(string message)
    {
        var cmd = ssoConsoleHelper.Value.FirstOrDefault(x => x.command == message);

        if (cmd != null)
        {
            if (cmd.command == "/Help")
            {
                UpdateUI("Commands", true);

                for (int i = 0; i < ssoConsoleHelper.Value.Count; i++)
                {
                    UpdateUI("- " + ssoConsoleHelper.Value[i].command + " => " + ssoConsoleHelper.Value[i].description.GetLocalizedString(), false);
                }
            }
            else if (cmd.command == "/Immortal")
            {
                if (!rsoConsoleCheats.Value.cantDie)
                {
                    rsoConsoleCheats.Value.cantDie = true;
                    UpdateUI("Your are now Immortal!", true);
                }
                else
                {
                    rsoConsoleCheats.Value.cantDie = false;
                    UpdateUI("Your are not Immortal!", true);
                }
            }
            else if (cmd.command == "/InfinitConviction")
            {
                if (!rsoConsoleCheats.Value.infiniteConviction)
                {
                    rsoConsoleCheats.Value.infiniteConviction = true;
                    UpdateUI("Your Conviction is Infinit!", true);
                }
                else
                {
                    rsoConsoleCheats.Value.infiniteConviction = false;
                    UpdateUI("Your Conviction is not Infinit!", true);
                }   
            }
            else if (cmd.command == "/Invincible")
            {
                if (!rsoConsoleCheats.Value.cantGetttingHit)
                {
                    rsoConsoleCheats.Value.cantGetttingHit = true;
                    UpdateUI("Your are now Invincible!", true);
                }
                else
                {
                    rsoConsoleCheats.Value.cantGetttingHit = false;
                    UpdateUI("Your are not Invincible!", true);
                }
            }
        }
        else
        {
            UpdateUI(message, true);
        }
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