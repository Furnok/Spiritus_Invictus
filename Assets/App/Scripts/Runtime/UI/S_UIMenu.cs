using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_UIMenu : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Windows")]
    [SerializeField] private GameObject settingsWindow;

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
    [SerializeField] private RSO_Navigation rsoNavigation;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InGame rsoInGame;

    public void ResumeGame()
    {
        rseOnGameInputEnabled.Call();
        rseOnCloseAllWindows.Call();
        rsoNavigation.Value.selectableFocus = null;
        rsoInGame.Value = true;
        rseOnGamePause.Call(false);
    }

    public void Settings()
    {
        rsoNavigation.Value.selectableFocus = null;
        rseOnOpenWindow.Call(settingsWindow);
    }

    public void MainMenu()
    {
        rseOnCloseAllWindows.Call();
        rsoNavigation.Value.selectableFocus = null;

        Scene currentScene = SceneManager.GetActiveScene();
        rseOnLoadScene.Call(currentScene.name);
    }

    public void QuitGame()
    {
        rseOnQuitGame.Call();
        rsoNavigation.Value.selectableFocus = null;
    }
}