using Playmode.Application;
using Playmode.Util.Values;
using UnityEngine;

namespace MenuEvent
{
	public class LoadMenuSceneOnClick : MonoBehaviour
	{
		public void LoadScene(string sceneName)
		{
			//BEN_CORRECTION : Tout les Find et GetComponents devraient être fait dans le Awake.
			//				   Par convention et par souci de performance.
			GameObject.FindGameObjectWithTag(Tags.MainController).GetComponent<MainController>().StopGame();
		}
	}
}