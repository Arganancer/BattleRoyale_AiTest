using Playmode.Event;
using Playmode.Interface;
using Playmode.Interface.VisualInterface;
using Playmode.Npc;
using Playmode.Util.Values;
using UnityEngine;
using UnityEngine.UI;

namespace Playmode.Application
{
	public class GameController : MonoBehaviour
	{
		private NpcDeathEventChannel npcDeathEventChannel;

		private GameObject[] pauseObjects;
		private GameObject[] endGameObjects;
		
		//TODO: clean
		private NpcController lastNpc;
		private Text endGameDetails;

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
			npcDeathEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<NpcDeathEventChannel>();

			isGamePaused = false;
			numberOfNpcs = GameValues.NbOfEnemies;
			
			pauseObjects = GameObject.FindGameObjectsWithTag(Tags.ShowOnPause);
			endGameObjects = GameObject.FindGameObjectsWithTag(Tags.ShowOnEnd);

			//TODO: clean
			//gameDetails = GameObject.FindGameObjectWithTag(Tags.ShowOnPause).GetComponent<Text>();
			//endGameTextDetails = GameObject.FindWithTag(Tags.ShowOnEnd).GetComponent<EndGameTextDetails>();
			//endGameDetails = GetComponentInChildren<Text>();

			endGameDetails = GameObject.FindGameObjectWithTag(Tags.EndGameDetails).GetComponent<Text>();

			UnpauseGame();
			StartGame();
		}

		private void OnEnable()
		{
			npcDeathEventChannel.OnEventPublished += DecrementNumberOfNpcs;
		}

		private void Update()
		{
			if (IsGameOver)
			{
				EndGame();
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (!isGamePaused)
				{
					isGamePaused = true;
					PauseGame();
				}
				else
				{
					isGamePaused = false;
					UnpauseGame();
				}
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
			Time.timeScale = 0.0f;
			
			foreach(GameObject g in pauseObjects)
			{
				g.SetActive(true);
			}
		}

		private void UnpauseGame()
		{
			Time.timeScale = 1.0f;
			
			foreach(GameObject g in pauseObjects)
			{
				g.SetActive(false);
			}
		}

		private void StartGame()
		{
			foreach(GameObject g in endGameObjects)
			{
				g.SetActive(false);
			}
		}

		private void EndGame()
		{
			Time.timeScale = 0.0f;

			foreach(GameObject g in endGameObjects)
			{
				g.SetActive(true);
			}

			if (numberOfNpcs == 0)
			{
				endGameDetails.text = "No survivor !";
			}
			else
			{
				lastNpc = GameObject.FindGameObjectWithTag(Tags.Npc).GetComponentInChildren<NpcController>();

				endGameDetails.text = lastNpc.GetHealth().ToString();
			}
			
			//endGameDetails.text = lastNpc.GetComponentInChildren<NpcController>().GetHealth().ToString();
			//endGameDetails.text = uiNpcCardController.lastNpcController.GetHealth().ToString();
		}
	}
}