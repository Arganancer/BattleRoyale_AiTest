using System.Collections;
using Playmode.Util;
using Playmode.Util.Values;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playmode.Application
{
    public class MainController : MonoBehaviour
    {
        private void Start()
        {
            LoadGameScene();
        }

        private void LoadGameScene()
        {
            StartCoroutine(LoadGameSceneRoutine());
        }

        public void ReloadGameScene()
        {
            StartCoroutine(ReloadGameSceneRoutine());
        }

        private static IEnumerator LoadGameSceneRoutine()
        {
            if (!SceneManager.GetSceneByName(Scenes.Game).isLoaded)
                yield return SceneManager.LoadSceneAsync(Scenes.Game, LoadSceneMode.Additive);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(Scenes.Game));
        }

        private static IEnumerator UnloadGameSceneRoutine()
        {
            if (SceneManager.GetSceneByName(Scenes.Game).isLoaded)
                yield return SceneManager.UnloadSceneAsync(Scenes.Game);
        }

        private static IEnumerator ReloadGameSceneRoutine()
        {
            yield return UnloadGameSceneRoutine();
            yield return LoadGameSceneRoutine();
        }
    }
}