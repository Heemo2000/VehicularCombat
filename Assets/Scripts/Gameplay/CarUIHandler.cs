using UnityEngine;
using Game.BaseUI;
using Game.Core;

namespace Game.Gameplay
{
    public class CarUIHandler : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private Page loadingPage;
        [SerializeField] private BarsUI loadingBar;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private string gameSceneName = "Game";

        public string GameSceneName { get => gameSceneName; set => gameSceneName = value; }

        public void Play()
        {
            sceneLoader.LoadScene(gameSceneName,
                                  () => uiManager.PushPage(loadingPage),
                                  loadingBar.SetAmount,
                                  uiManager.PopPage);
        }
    }
}
