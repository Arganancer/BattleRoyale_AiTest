using Playmode.Event;
using Playmode.Npc;
using Playmode.Util.Values;
using UnityEngine;
using UnityEngine.UI;

namespace Playmode.Application
{
	public class GameController : MonoBehaviour
	{
		private const string TextNoSurvivors = "No survivors !";
		private const string TextSurvivorInfo = "Survivor's remaining health points : ";
		
		private NpcDeathEventChannel npcDeathEventChannel;
		private GameObject pauseObjects;
		private GameObject endGameObjects;
		private NpcController lastNpc;
		private Text endGameDetails;
		private const float RegularTimeSpeed = 0.5f; // TODO: Change to 1.0f for final push;
		private const float PausedTimeSpeed = 0.0f;

		private int numberOfNpcs;
		private bool isGamePaused;

		private bool IsGameOver => numberOfNpcs < 2;

		private void Awake()
		{
			npcDeathEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<NpcDeathEventChannel>();
			pauseObjects = GameObject.FindGameObjectWithTag(Tags.ShowOnPause);
			endGameObjects = GameObject.FindGameObjectWithTag(Tags.ShowOnEnd);
			endGameDetails = GameObject.FindGameObjectWithTag(Tags.EndGameDetails).GetComponent<Text>();

			isGamePaused = false;
			numberOfNpcs = GameValues.NbOfEnemies;
			
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
			Time.timeScale = PausedTimeSpeed;
			
			pauseObjects.SetActive(true);
		}

		private void UnpauseGame()
		{
			Time.timeScale = RegularTimeSpeed;
			
			pauseObjects.SetActive(false);
		}

		private void StartGame()
		{
			endGameObjects.SetActive(false);
		}

		private void EndGame()
		{
			Time.timeScale = PausedTimeSpeed;

			endGameObjects.SetActive(true);

			if (numberOfNpcs == 0)
			{
				endGameDetails.text = TextNoSurvivors;
			}
			else
			{
				lastNpc = GameObject.FindGameObjectWithTag(Tags.Npc).GetComponentInChildren<NpcController>();

				endGameDetails.text = TextSurvivorInfo;
				endGameDetails.text += lastNpc.GetHealth().ToString();
			}
		}
	}
}