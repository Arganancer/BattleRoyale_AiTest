using UnityEngine;
using UnityEngine.UI;

namespace Playmode.Application
{
	public class MenuController : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<Image>().color = Color.black;
		}
	}
}