using Playmode.Event;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Application
{
	public class GameController : MonoBehaviour
	{
		private MainController mainController;
		private NpcDeathEventChannel npcDeathEventChannel;

		private void Awake()
		{
			mainController = GameObject.FindWithTag(Tags.MainController).GetComponent<MainController>();
			npcDeathEventChannel = GetComponent<NpcDeathEventChannel>();
		}

		private void OnEnable()
		{
			throw new System.NotImplementedException();
		}

		private void Update()
		{
			throw new System.NotImplementedException();
		}

		private void OnDisable()
		{
			throw new System.NotImplementedException();
		}
	}
}