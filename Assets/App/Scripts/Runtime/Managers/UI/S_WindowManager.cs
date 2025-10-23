using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class S_WindowManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Windows")]
    [SerializeField] private GameObject mainMenuWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject menuWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject gameWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnOpenWindow rseOnOpenWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCloseWindow rseOnCloseWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCloseAllWindows rseOnCloseAllWindows;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDisplayUIGame rseOnDisplayUIGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUIInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGamePause rseOnGamePause;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InGame rsoInGame;

    private List<GameObject> currentWindows = new();

    private void Awake()
    {
        rsoInGame.Value = true;
    }

    private void OnEnable()
    {
        rseOnOpenWindow.action += AlreadyOpen;
        rseOnCloseWindow.action += CloseWindow;
        rseOnCloseAllWindows.action += CloseAllWindows;
        rseOnPlayerPause.action += PauseGame;
        rseOnDisplayUIGame.action += DisplayUIGame;
    }

    private void OnDisable()
    {
        rseOnOpenWindow.action -= AlreadyOpen;
        rseOnCloseWindow.action -= CloseWindow;
        rseOnCloseAllWindows.action -= CloseAllWindows;
        rseOnPlayerPause.action -= PauseGame;
        rseOnDisplayUIGame.action -= DisplayUIGame;
    }

    private void DisplayUIGame(bool value)
    {
        gameWindow.SetActive(value);
    }

    private void PauseGame()
    {
        if (rsoInGame.Value)
        {
            if (currentWindows.Count < 2)
            {
                if (!menuWindow.activeInHierarchy)
                {
                    rseOnUIInputEnabled.Call();
                    OpenWindow(menuWindow);
                    rseOnGamePause.Call(true);
                }
                else
                {
                    rseOnGameInputEnabled.Call();
                    CloseWindow(menuWindow);
                    rseOnGamePause.Call(false);
                }
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
}