using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class S_WindowManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Default Window")]
    [SerializeField] private GameObject defaultWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnOpenWindow rseOnOpenWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCloseWindow rseOnCloseWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCloseAllWindows rseOnCloseAllWindows;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnQuitGame rseOnQuitGame;

    private List<GameObject> currentWindows = new();

    private void OnEnable()
    {
        rseOnOpenWindow.action += AlreadyOpen;
        rseOnCloseWindow.action += CloseWindow;
        rseOnCloseAllWindows.action += CloseAllWindows;
        rseOnPlayerPause.action += PauseGame;
    }

    private void OnDisable()
    {
        rseOnOpenWindow.action -= AlreadyOpen;
        rseOnCloseWindow.action -= CloseWindow;
        rseOnCloseAllWindows.action -= CloseAllWindows;
        rseOnPlayerPause.action -= PauseGame;
    }

    private void Start()
    {
        if (defaultWindow != null)
        {
            OpenWindow(defaultWindow);
        }
    }

    private void PauseGame()
    {
        rseOnQuitGame.Call();
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