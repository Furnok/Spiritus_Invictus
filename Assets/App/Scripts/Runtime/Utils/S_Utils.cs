using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class S_Utils
{
    #region COROUTINE
    public static IEnumerator Delay(float delay, Action onComplete = null)
    {
        yield return new WaitForSeconds(delay);

        onComplete?.Invoke();
    }

    public static IEnumerator DelayFrame(Action onComplete = null)
    {
        yield return null;

        onComplete?.Invoke();
    }

    public static IEnumerator DelayRealTime(float delay, Action onComplete = null)
    {
        yield return new WaitForSecondsRealtime(delay);

        onComplete?.Invoke();
    }
    #endregion

    #region SCENE
    public static IEnumerator LoadSceneAsync(int sceneIndex, LoadSceneMode loadMode, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, loadMode);

        yield return new WaitUntil(() => asyncLoad.isDone);

        onComplete?.Invoke();
    }

    public static IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadMode, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadMode);

        yield return new WaitUntil(() => asyncLoad.isDone);

        onComplete?.Invoke();
    }

    public static IEnumerator UnloadSceneAsync(string sceneName, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);

        yield return new WaitUntil(() => asyncLoad.isDone);

        onComplete?.Invoke();
    }

    public static IEnumerator UnloadSceneAsync(int sceneIndex, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneIndex);

        yield return new WaitUntil(() => asyncLoad.isDone);

        onComplete?.Invoke();
    }
    #endregion
}