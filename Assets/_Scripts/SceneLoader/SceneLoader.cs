using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private MonoBehaviour _coroutineHolder;

    public SceneLoader(MonoBehaviour coroutineHolder)
    {
        _coroutineHolder = coroutineHolder;
    }

    public void LoadSceneAsync(string sceneName, Action onLoadStarted = null, Action onLoadEnded = null)
    {
        _coroutineHolder.StartCoroutine(SceneLoadRoutine(sceneName, onLoadStarted, onLoadEnded));
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private IEnumerator SceneLoadRoutine(string sceneName, Action onLoadStarted = null, Action onLoadEnded = null)
    {
        onLoadStarted?.Invoke();

        if (SceneManager.GetActiveScene().name == sceneName)
        {
            onLoadEnded?.Invoke();
            yield break;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
            yield return null;

        onLoadEnded?.Invoke();
    }
}
