using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName, 
                              Action OnSceneLoadStarts, 
                              Action<float> OnSceneLoading, 
                              Action OnSceneLoaded)
        {
            StartCoroutine(LoadSceneAsync(sceneName,
                                          OnSceneLoadStarts,
                                          OnSceneLoading,
                                          OnSceneLoaded));
        }

        private IEnumerator LoadSceneAsync(string sceneName,
                                    Action OnSceneLoadStarts,
                                    Action<float> OnSceneLoading,
                                    Action OnSceneLoaded)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

            OnSceneLoadStarts?.Invoke();
            yield return null;
            while(loadOperation.progress < 1.0f)
            {
                yield return null;
                OnSceneLoading?.Invoke(loadOperation.progress);
            }

            OnSceneLoaded?.Invoke();
        }
    }
}
