using System.Collections;
using Playmode.Util.Values;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playmode.Application
{
	public class MainController : MonoBehaviour
	{
		[SerializeField] private static float TimeScale = 1.0f;

		private void Start()
		{
			SetTimeScale();
			StartCoroutine(LoadMenuSceneRoutine());
		}

		public void StartGame()
		{
			StartCoroutine(LoadGameSceneRoutine());
		}

		public void StopGame()
		{
			StartCoroutine(UnloadGameSceneRoutine());
		}

		public void PauseGame()
		{
			Time.timeScale = 0.0f;
			SetTimeScale();
		}

		public void UnpauseGame()
		{
			Time.timeScale = 1.0f;
			SetTimeScale();
		}

		private static void SetTimeScale()
		{
			// TODO: Remove this function.
			// Used to slow down global game time for testing purposes.
			// Regular timeScale for normal speed is 1.0f;
			Time.timeScale = TimeScale;
		}

		private static IEnumerator LoadGameSceneRoutine()
		{
			if (SceneManager.GetSceneByName(Scenes.Menu).isLoaded)
				yield return SceneManager.UnloadSceneAsync(Scenes.Menu);
			
			if (!SceneManager.GetSceneByName(Scenes.Game).isLoaded)
				yield return SceneManager.LoadSceneAsync(Scenes.Game, LoadSceneMode.Additive);

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(Scenes.Game));
		}

		private static IEnumerator UnloadGameSceneRoutine()
		{
			if (SceneManager.GetSceneByName(Scenes.Game).isLoaded)
				yield return SceneManager.UnloadSceneAsync(Scenes.Game);

			if (!SceneManager.GetSceneByName(Scenes.Menu).isLoaded)
				yield return SceneManager.LoadSceneAsync(Scenes.Menu, LoadSceneMode.Additive);

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(Scenes.Menu));
		}
		
		private static IEnumerator LoadMenuSceneRoutine()
		{
			if (!SceneManager.GetSceneByName(Scenes.Menu).isLoaded)
				yield return SceneManager.LoadSceneAsync(Scenes.Menu, LoadSceneMode.Additive);

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(Scenes.Menu));
		}
	}
}