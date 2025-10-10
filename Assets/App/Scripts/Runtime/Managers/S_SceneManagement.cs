using UnityEngine;
using UnityEngine.SceneManagement;

public class S_SceneManagement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private S_SceneReference[] levelsName;

    [Header("Inputs")]
    [SerializeField] private RSE_OnLoadScene rseOnLoadScene;
    [SerializeField] private RSE_OnQuitGame rseOnQuitGame;

    [Header("Output")]
    [SerializeField] private RSO_CurrentLevel rsoCurrentLevel;

    private bool isLoading = false;

    private void OnEnable()
    {
        rseOnLoadScene.action += LoadLevel;
        rseOnQuitGame.action += QuitGame;
    }

    private void OnDisable()
    {
        rseOnLoadScene.action -= LoadLevel;
        rseOnQuitGame.action -= QuitGame;

        rsoCurrentLevel.Value = null;
    }

    private void LoadLevel(string sceneName)
    {
        if (isLoading) return;

        isLoading = true;

        Transition(sceneName);
    }

    private void Transition(string sceneName)
    {
        if (rsoCurrentLevel.Value != null)
        {
            StartCoroutine(S_Utils.UnloadSceneAsync(rsoCurrentLevel.Value));
        }

        StartCoroutine(S_Utils.LoadSceneAsync(sceneName, LoadSceneMode.Additive, () =>
        {
            isLoading = false;

            rsoCurrentLevel.Value = sceneName;
        }));
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}