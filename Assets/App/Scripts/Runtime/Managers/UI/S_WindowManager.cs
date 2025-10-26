using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_WindowManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Windows")]
    [SerializeField] private GameObject menuWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject gameWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject fadeWindow;

    [TabGroup("References")]
    [Title("Images")]
    [SerializeField] private Image imageFade;

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
    [SerializeField] private RSO_GameInPause rsoGameInPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InGame rsoInGame;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_FadeTime ssoFadeTime;

    private List<GameObject> currentWindows = new();

    private void Awake()
    {
        rsoInGame.Value = true;
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
        gameWindow.SetActive(value);
    }

    private void PauseGame()
    {
        if (rsoInGame.Value && currentWindows.Count < 1)
        {
            if (!menuWindow.activeInHierarchy)
            {
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
        window.SetActive(true);

        currentWindows.Add(window);
    }

    private void CloseWindow(GameObject window)
    {
        if (window != null && window.activeInHierarchy)
        {
            window.SetActive(false);

            currentWindows.Remove(window);
        }
    }

    private void CloseAllWindows()
    {
        foreach (var window in currentWindows)
        {
            window.SetActive(false);
        }

        currentWindows.Clear();
    }

    private void FadeIn()
    {
        imageFade?.DOKill();

        imageFade.DOFade(0f, ssoFadeTime.Value).SetEase(Ease.Linear);
    }

    private void FadeOut()
    {
        imageFade?.DOKill();

        imageFade.DOFade(1f, ssoFadeTime.Value).SetEase(Ease.Linear);
    }
}