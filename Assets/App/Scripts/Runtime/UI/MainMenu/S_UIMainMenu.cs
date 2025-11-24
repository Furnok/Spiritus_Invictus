using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_UIMainMenu : MonoBehaviour
{
    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeFadeSkip;

    [TabGroup("References")]
    [Title("Buttons")]
    [SerializeField] private Button buttonStart;

    [TabGroup("References")]
    [SerializeField] private Button buttonContinue;

    [TabGroup("References")]
    [SerializeField] private Button buttonSettings;

    [TabGroup("References")]
    [Title("Windows")]
    [SerializeField] private GameObject settingsWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject creditsWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDataTemp rseOnDataTemp;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUIInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCameraIntro rseOnCameraIntro;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnOpenWindow rseOnOpenWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCloseAllWindows rseOnCloseAllWindows;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnQuitGame rseOnQuitGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnDisplayUIGame rseOnDisplayUIGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnFadeOut rseOnFadeOut;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnShowMouseCursor rseOnShowMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnHideMouseCursor rseOnHideMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InGame rsoInGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_DataTempSaved rsoDataTempSaved;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_FadeTime ssoFadeTime;

    private bool isTransit = true;

    private void OnEnable()
    {
        rseOnDataTemp.action += SetupMenu;

        StartCoroutine(S_Utils.DelayFrame(() =>
        {
            if (Gamepad.current == null)
            {
               rseOnShowMouseCursor.Call();
            }

            rseOnUIInputEnabled.Call();
            rsoInGame.Value = false;
            rseOnDisplayUIGame.Call(false);
        }));

        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        cg.DOKill();

        StartCoroutine(S_Utils.DelayRealTime(ssoFadeTime.Value, () =>
        {
            cg.DOFade(1f, timeFadeSkip).SetEase(Ease.Linear);
        }));

        isTransit = false;
    }

    private void OnDisable()
    {
        rseOnDataTemp.action -= SetupMenu;

        isTransit = false;
    }

    private void SetupMenu()
    {
        if (rsoDataTempSaved.Value.haveSave)
        {
            buttonContinue.gameObject.SetActive(true);

            Navigation nav = buttonStart.navigation;
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnDown = buttonContinue;

            buttonStart.navigation = nav;

            Navigation nav2 = buttonSettings.navigation;
            nav2.mode = Navigation.Mode.Explicit;

            nav2.selectOnUp = buttonContinue;

            buttonSettings.navigation = nav2;
        }
    }

    public void StartGame()
    {
        if (!isTransit)
        {
            isTransit = true;

            rseOnHideMouseCursor.Call();

            rseOnCloseAllWindows.Call();

            CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
            cg.DOKill();

            cg.DOFade(0f, timeFadeSkip).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
            {
                gameObject.SetActive(false);
                rsoNavigation.Value.selectableFocus = null;
                rseOnCameraIntro.Call();
                rsoInGame.Value = true;
            });
        }
    }

    public void ContinueGame()
    {
        if (!isTransit)
        {
            isTransit = true;

            rseOnHideMouseCursor.Call();

            rseOnFadeOut.Call();

            StartCoroutine(S_Utils.DelayRealTime(ssoFadeTime.Value, () =>
            {
                rseOnCloseAllWindows.Call();
                rsoNavigation.Value.selectableFocus = null;
            }));
        }
    }

    public void Settings()
    {
        if (!isTransit)
        {
            rseOnCloseAllWindows.Call();
            rsoNavigation.Value.selectableFocus = null;
            rseOnOpenWindow.Call(settingsWindow);
        }
    }

    public void Credits()
    {
        if (!isTransit)
        {
            rseOnCloseAllWindows.Call();
            rsoNavigation.Value.selectableFocus = null;
            rseOnOpenWindow.Call(creditsWindow);
        }
    }

    public void QuitGame()
    {
        if (!isTransit)
        {
            isTransit = true;

            rseOnFadeOut.Call();

            StartCoroutine(S_Utils.DelayRealTime(ssoFadeTime.Value, () =>
            {
                rseOnQuitGame.Call();
                rsoNavigation.Value.selectableFocus = null;
            }));
        }
    }
}