using Playmode.Event;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Application
{
	public class GameController : MonoBehaviour
	{
		private MainController mainController;
		//private SoundController soundController;
		private NpcDeathEventChannel npcDeathEventChannel;

		private int numberOfNpcs;
		private bool isGamePaused;
		
		public int NumberOfNpcs
		{
			get { return numberOfNpcs; }
			set
			{
				if (numberOfNpcs != value)
					numberOfNpcs = value;
			}
		}

		public bool IsGameOver => numberOfNpcs < 2;

		public bool IsGamePaused
		{
			get { return isGamePaused; }
			set
			{
				if (isGamePaused != value)
					isGamePaused = value;
			}
		}

		private void Awake()
		{
			mainController = GameObject.FindWithTag(Tags.MainController).GetComponent<MainController>();
			//soundController = GameObject.FindWithTag(Tags.SoundController).GetComponent<SoundController>();
			npcDeathEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<NpcDeathEventChannel>();

			NumberOfNpcs = 9; //TODO: change for menu option
		}

		private void OnEnable()
		{
			npcDeathEventChannel.OnEventPublished += DecrementNumberOfNpcs;
		}

		private void Update()
		{
			if (IsGameOver)
			{
				GameObject.FindGameObjectWithTag(Tags.MainController).GetComponent<MainController>().StopGame();
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				PauseGame();
			}
		}

		private void OnDisable()
		{
			npcDeathEventChannel.OnEventPublished -= DecrementNumberOfNpcs;
		}

		private void DecrementNumberOfNpcs()
		{
			numberOfNpcs--;
		}

		private void PauseGame()
		{
			GameObject.FindGameObjectWithTag(Tags.MainController).GetComponent<MainController>().PauseGame();	
		}
	}
}