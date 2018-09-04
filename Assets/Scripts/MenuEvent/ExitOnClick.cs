using UnityEngine;

namespace MenuEvent
{
	public class ExitOnClick : MonoBehaviour
	{
		public void ExitGame()
		{
			Application.Quit();
		}
	}
}