using Playmode.Application;
using Playmode.Util.Values;
using UnityEngine;

namespace MenuEvent
{
	public class LoadMenuSceneOnClick : MonoBehaviour
	{
		public void LoadScene(string sceneName)
		{
			GameObject.FindGameObjectWithTag(Tags.MainController).GetComponent<MainController>().StopGame();
		}
	}
}