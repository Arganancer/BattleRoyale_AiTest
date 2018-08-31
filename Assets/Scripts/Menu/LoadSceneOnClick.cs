using Playmode.Application;
using Playmode.Util.Values;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
	public class LoadSceneOnClick : MonoBehaviour
	{
		public void LoadScene(string sceneName)
		{
			GameObject.FindGameObjectWithTag(Tags.MainController).GetComponent<MainController>().StartGame();
		}
	}
}