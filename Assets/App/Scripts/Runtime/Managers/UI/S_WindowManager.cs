using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_WindowManager : MonoBehaviour
{
    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeFade;

    [TabGroup("References")]
    [Title("Audio")]
    [SerializeField] private EventReference uiSound;

    [TabGroup("References")]
    [Title("Windows")]
    [SerializeField] private GameObject menuWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject gameWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject consoleBackgroundWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject fadeWindow;

    [TabGroup("References")]
    [Title("Images")]
    [SerializeField] private Image imageFade;

    [TabGroup("References")]
    [Title("Console")]
    [SerializeField] private TMP_InputField inputField;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnOpenWindow rseOnOpenWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCloseWindow rseOnCloseWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCloseAllWindows rseOnCloseAllWindows;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnFadeIn rseOnFadeIn;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnFadeOut rsOnFadeOut;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDisplayUIGame rseOnDisplayUIGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUIInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGamePause rseOnGamePause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnShowMouseCursor rseOnShowMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnHideMouseCursor rseOnHideMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_GameInPause rsoGameInPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InGame rsoInGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_CurrentWindows rsoCurrentWindows;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InConsole rsoInConsole;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_FadeTime ssoFadeTime;

    private void Awake()
    {
        rsoInGame.Value = true;
        rsoCurrentWindows.Value = new();
        fadeWindow.SetActive(true);
    }

    private void OnEnable()
    {
        rseOnOpenWindow.action += AlreadyOpen;
        rseOnCloseWindow.action += CloseWindow;
        rseOnCloseAllWindows.action += CloseAllWindows;
        rseOnPlayerPause.action += PauseGame;
        rseOnFadeIn.action += FadeIn;
        rsOnFadeOut.action += FadeOut;
        rseOnDisplayUIGame.action += DisplayUIGame;
    }

    private void OnDisable()
    {
        rseOnOpenWindow.action -= AlreadyOpen;
        rseOnCloseWindow.action -= CloseWindow;
        rseOnCloseAllWindows.action -= CloseAllWindows;
        rseOnPlayerPause.action -= PauseGame;
        rseOnFadeIn.action -= FadeIn;
        rsOnFadeOut.action -= FadeOut;
        rseOnDisplayUIGame.action -= DisplayUIGame;

        imageFade?.DOKill();
    }

    private void Start()
    {
        StartCoroutine(S_Utils.DelayFrame(() => FadeIn()));
    }

    private void DisplayUIGame(bool value)
    {
        gameWindow.GetComponent<CanvasGroup>()?.DOKill();

        if (value && !gameWindow.activeInHierarchy)
        {
            gameWindow.gameObject.SetActive(true);

            gameWindow.GetComponent<CanvasGroup>().alpha = 0f;
            gameWindow.GetComponent<CanvasGroup>().DOFade(1f, timeFade).SetEase(Ease.Linear);
        }
        else if (!value)
        {
            gameWindow.GetComponent<CanvasGroup>().alpha = 1f;
            gameWindow.GetComponent<CanvasGroup>().DOFade(0f, timeFade).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameWindow.SetActive(false);
            });
        }
    }

    private void PauseGame()
    {
        if (inputField.isFocused)
        {
            return;
        }

        if (rsoInConsole.Value)
        {
            rseOnHideMouseCursor.Call();

            if (rsoGameInPause.Value)
            {
                rseOnUIInputEnabled.Call();

                rseOnShowMouseCursor.Call();
            }
            else
            {
                rseOnGameInputEnabled.Call();
            }
            consoleBackgroundWindow.GetComponent<CanvasGroup>().alpha = 1f;
            consoleBackgroundWindow.GetComponent<CanvasGroup>().DOFade(0f, timeFade).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
            {
                consoleBackgroundWindow.GetComponent<CanvasGroup>().blocksRaycasts = false;
            });

            StartCoroutine(S_Utils.DelayFrame(() => rsoInConsole.Value = false));

            return;
        }

        if (rsoInGame.Value && rsoCurrentWindows.Value.Count < 1)
        {
            if (!menuWindow.activeInHierarchy)
            {
                RuntimeManager.PlayOneShot(uiSound);

                rseOnUIInputEnabled.Call();
                OpenWindow(menuWindow);
                rsoGameInPause.Value = true;
                rseOnGamePause.Call(true);
            }
        }
    }

    private void AlreadyOpen(GameObject window)
    {
        if (window != null)
        {
            if (!window.activeInHierarchy)
            {
                OpenWindow(window);
            }
            else
            {
                CloseWindow(window);
            }
        }
    }

    private void OpenWindow(GameObject window)
    {
        window.GetComponent<CanvasGroup>()?.DOKill();

        window.SetActive(true);

        window.GetComponent<CanvasGroup>().alpha = 0f;
        window.GetComponent<CanvasGroup>().DOFade(1f, timeFade).SetEase(Ease.Linear).SetUpdate(true);

        rsoCurrentWindows.Value.Add(window);
    }

    private void CloseWindow(GameObject window)
    {
        if (window != null && window.activeInHierarchy)
        {
            window.GetComponent<CanvasGroup>()?.DOKill();

            window.GetComponent<CanvasGroup>().alpha = 1f;
            window.GetComponent<CanvasGroup>().DOFade(0f, timeFade).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
            {
                window.SetActive(false);
            });

            rsoCurrentWindows.Value.Remove(window);
        }
    }

    private void CloseAllWindows()
    {
        foreach (var window in rsoCurrentWindows.Value)
        {
            window.SetActive(false);
        }

        rsoCurrentWindows.Value.Clear();
    }

    private void FadeIn()
    {
        imageFade?.DOKill();

        imageFade.DOFade(0f, ssoFadeTime.Value).SetEase(Ease.Linear).SetUpdate(true);
    }

    private void FadeOut()
    {
        imageFade?.DOKill();

        imageFade.DOFade(1f, ssoFadeTime.Value).SetEase(Ease.Linear).SetUpdate(true);
    }
}