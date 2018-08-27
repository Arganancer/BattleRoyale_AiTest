using System.Collections;
using Playmode.Util.Values;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playmode.Application
{
	public class MainController : MonoBehaviour
	{
		[SerializeField] private static float TimeScale = 0.4f;

		private void Start()
		{
			SetTimeScale();
			LoadGameScene();
		}

		private static void SetTimeScale()
		{
			// TODO: Remove this function.
			// Used to slow down global game time for testing purposes.
			// Regular timeScale for normal speed is 1.0f;
			Time.timeScale = TimeScale;
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