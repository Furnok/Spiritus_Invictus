using Sirenix.OdinInspector;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public class S_UIMainMenu : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("TEMP ENABLE CONTINUE")]
    [SerializeField] private bool isActive;

    [TabGroup("References")]
    [Title("Buttons")]
    [SerializeField] private Button buttonStart;

    [TabGroup("References")]
    [SerializeField] private Button buttonContinue;

    [TabGroup("References")]
    [SerializeField] private Button buttonSettings;

    [TabGroup("References")]
    [SerializeField] private GameObject settingsWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject creditsWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUIInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnOpenWindow rseOnOpenWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCloseAllWindows rseOnCloseAllWindows;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnQuitGame rseOnQuitGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    private void OnEnable()
    {
        StartCoroutine(S_Utils.DelayFrame(() => rseOnUIInputEnabled.Call()));

        if (isActive)
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
        rseOnGameInputEnabled.Call();
    }

    public void ContinueGame()
    {
        rseOnCloseAllWindows.Call();
        rsoNavigation.Value.selectableFocus = null;
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
        rseOnQuitGame.Call();
        rsoNavigation.Value.selectableFocus = null;
    }
}