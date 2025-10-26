using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class S_UIMainMenu : MonoBehaviour
{
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
    [SerializeField] private RSO_Navigation rsoNavigation;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InGame rsoInGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_DataTempSaved rsoDataTempSaved;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_FadeTime ssoFadeTime;

    private void OnEnable()
    {
        rseOnDataTemp.action += SetupMenu;

        StartCoroutine(S_Utils.DelayFrame(() => rseOnUIInputEnabled.Call()));
        StartCoroutine(S_Utils.DelayFrame(() => rsoInGame.Value = false));
        StartCoroutine(S_Utils.DelayFrame(() => rseOnDisplayUIGame.Call(false)));
    }

    private void OnDisable()
    {
        rseOnDataTemp.action -= SetupMenu;
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
        rseOnCloseAllWindows.Call();
        gameObject.SetActive(false);
        rsoNavigation.Value.selectableFocus = null;
        rseOnCameraIntro.Call();
        rsoInGame.Value = true;
    }

    public void ContinueGame()
    {
        rseOnFadeOut.Call();

        StartCoroutine(S_Utils.Delay(ssoFadeTime.Value, () =>
        {
            rseOnCloseAllWindows.Call();
            rsoNavigation.Value.selectableFocus = null;
        }));
    }

    public void Settings()
    {
        rseOnCloseAllWindows.Call();
        rsoNavigation.Value.selectableFocus = null;
        rseOnOpenWindow.Call(settingsWindow);
    }

    public void Credits()
    {
        rseOnCloseAllWindows.Call();
        rsoNavigation.Value.selectableFocus = null;
        rseOnOpenWindow.Call(creditsWindow);
    }

    public void QuitGame()
    {
        rseOnFadeOut.Call();

        StartCoroutine(S_Utils.Delay(ssoFadeTime.Value, () =>
        {
            rseOnQuitGame.Call();
            rsoNavigation.Value.selectableFocus = null;
        }));
    }
}