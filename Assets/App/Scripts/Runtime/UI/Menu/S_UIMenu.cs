using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_UIMenu : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Windows")]
    [SerializeField] private GameObject settingsWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnOpenWindow rseOnOpenWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCloseAllWindows rseOnCloseAllWindows;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnQuitGame rseOnQuitGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnLoadScene rseOnLoadScene;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGamePause rseOnGamePause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnResetFocus rseOnResetFocus;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnFadeOut rseOnFadeOut;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InGame rsoInGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_GameInPause rsoGameInPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_CurrentWindows rsoCurrentWindows;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_FadeTime ssoFadeTime;

    private void OnEnable()
    {
        rseOnPlayerPause.action += ResumeGame;
    }

    private void OnDisable()
    {
        rseOnPlayerPause.action -= ResumeGame;
    }

    public void ResumeGame()
    {
        if (rsoCurrentWindows.Value.Count < 2)
        {
            rseOnGameInputEnabled.Call();
            rseOnCloseAllWindows.Call();
            rsoNavigation.Value.selectableDefault = null;
            rseOnResetFocus.Call();
            rsoNavigation.Value.selectableFocus = null;
            rsoInGame.Value = true;
            rsoGameInPause.Value = false;
            rseOnGamePause.Call(false);
        }
    }

    public void Settings()
    {
        rsoNavigation.Value.selectableFocus = null;
        rseOnOpenWindow.Call(settingsWindow);
    }

    public void MainMenu()
    {
        rseOnFadeOut.Call();

        StartCoroutine(S_Utils.Delay(ssoFadeTime.Value, () =>
        {
            rseOnCloseAllWindows.Call();
            rsoNavigation.Value.selectableFocus = null;

            rsoGameInPause.Value = false;
            rseOnGamePause.Call(false);

            Scene currentScene = SceneManager.GetActiveScene();
            rseOnLoadScene.Call(currentScene.name);
        }));
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