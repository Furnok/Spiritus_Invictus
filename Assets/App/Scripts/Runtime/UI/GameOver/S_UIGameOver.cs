using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class S_UIGameOver : MonoBehaviour
{
    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCloseAllWindows rseOnCloseAllWindows;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnQuitGame rseOnQuitGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnLoadScene rseOnLoadScene;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGamePause rseOnGamePause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnFadeIn rseOnFadeIn;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnFadeOut rseOnFadeOut;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnShowMouseCursor rseOnShowMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnHideMouseCursor rseOnHideMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnResetFocus rseOnResetFocus;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerRespawn rseOnPlayerRespawn;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_FadeTime ssoFadeTime;

    private bool isTransit = false;

    private void OnEnable()
    {
        if (Gamepad.current == null)
        {
            rseOnShowMouseCursor.Call();
        }

        isTransit = false;
    }

    private void OnDisable()
    {
        isTransit = false;
    }

    public void Respawn()
    {
        if (!isTransit)
        {
            isTransit = true;
            rseOnFadeOut.Call();

            rseOnHideMouseCursor.Call();

            rseOnCloseAllWindows.Call();

            StartCoroutine(S_Utils.DelayRealTime(ssoFadeTime.Value, () =>
            {
                rseOnGameInputEnabled.Call();

                rseOnGamePause.Call(false);

                StartCoroutine(S_Utils.DelayRealTime(1.2f, () =>
                {
                    gameObject.SetActive(false);
                    rsoNavigation.Value.selectableDefault = null;
                    rseOnResetFocus.Call();

                    rseOnPlayerRespawn.Call();

                    rseOnFadeIn.Call();
                }));
            }));
        }
    }

    public void MainMenu()
    {
        if (!isTransit)
        {
            isTransit = true;
            rseOnFadeOut.Call();

            StartCoroutine(S_Utils.DelayRealTime(ssoFadeTime.Value, () =>
            {
                rseOnCloseAllWindows.Call();
                rsoNavigation.Value.selectableFocus = null;

                rseOnGamePause.Call(false);

                Scene currentScene = SceneManager.GetActiveScene();
                rseOnLoadScene.Call(currentScene.name);
            }));
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