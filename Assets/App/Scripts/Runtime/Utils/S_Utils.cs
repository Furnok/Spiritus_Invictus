using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public static class S_Utils
{
    #region COROUTINE
    public static IEnumerator Delay(float delay, Action onComplete = null)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        onComplete?.Invoke();
    }

    public static IEnumerator DelayFrame(Action onComplete = null)
    {
        yield return null;

        onComplete?.Invoke();
    }

    public static IEnumerator DelayRealTime(float delay, Action onComplete = null)
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);

        onComplete?.Invoke();
    }
    #endregion

    #region SCENE
    public static IEnumerator LoadSceneAsync(int sceneIndex, LoadSceneMode loadMode, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, loadMode);

        if (asyncLoad == null) yield break;

        yield return new WaitUntil(() => asyncLoad.isDone);

        onComplete?.Invoke();
    }

    public static IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadMode, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadMode);

        if (asyncLoad == null) yield break;

        yield return new WaitUntil(() => asyncLoad.isDone);

        onComplete?.Invoke();
    }

    public static IEnumerator UnloadSceneAsync(string sceneName, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);

        if (asyncLoad == null) yield break;

        yield return new WaitUntil(() => asyncLoad.isDone);

        onComplete?.Invoke();
    }

    public static IEnumerator UnloadSceneAsync(int sceneIndex, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneIndex);

        if (asyncLoad == null) yield break;

        yield return new WaitUntil(() => asyncLoad.isDone);

        onComplete?.Invoke();
    }
    #endregion

    #region Controller
    public static IEnumerator Shake(float lowFreq, float highFreq, float duration)
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(lowFreq, highFreq);

            yield return new WaitForSeconds(duration);

            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }
    #endregion
}