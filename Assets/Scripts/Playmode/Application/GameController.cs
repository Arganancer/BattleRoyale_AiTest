using Playmode.Sound;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Application
{
	public class GameController : MonoBehaviour
	{
		private MainController mainController;
		private SoundController soundController;

		private void Awake()
		{
			mainController = GameObject.FindWithTag(Tags.MainController).GetComponent<MainController>();
			soundController = GameObject.FindWithTag(Tags.SoundController).GetComponent<SoundController>();
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