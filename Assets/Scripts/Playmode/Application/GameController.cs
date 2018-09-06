using System;
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
		private const string TextSurvivorInfo = "Survivor's remaining HP : ";
		private const string TextTimeTaken = "Time taken :";
		private const float RegularTimeSpeed = 1.0f;
		private const float PausedTimeSpeed = 0.0f;

		private NpcDeathEventChannel npcDeathEventChannel;
		private GameObject pauseObjects;
		private GameObject endGameObjects;
		private NpcController lastNpc;
		private Text endGameDetails;

		private int numberOfNpcs;
		private bool isGamePaused;
		private float timePassedInSeconds;
		private string timePassed;

		private bool IsGameOver => numberOfNpcs < 2;

		private void Awake()
		{
			npcDeathEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<NpcDeathEventChannel>();
			pauseObjects = GameObject.FindGameObjectWithTag(Tags.ShowOnPause);
			endGameObjects = GameObject.FindGameObjectWithTag(Tags.ShowOnEnd);
			endGameDetails = GameObject.FindGameObjectWithTag(Tags.EndGameDetails).GetComponent<Text>();

			isGamePaused = false;
			numberOfNpcs = GameValues.NbOfEnemies;
			timePassedInSeconds = 0.0f;
			timePassed = "";
			
			UnpauseGame();
			StartGame();
		}

		private void OnEnable()
		{
			npcDeathEventChannel.OnEventPublished += DecrementNumberOfNpcs;
		}

		private void Update()
		{
			timePassedInSeconds += Time.deltaTime;
			
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
			ConvertTime();

			endGameObjects.SetActive(true);

			if (numberOfNpcs == 0)
			{
				GameDetailsNoSurvivors();
			}
			else
			{
				GameDetailsSurvivor();
			}
		}

		private void ConvertTime()
		{
			var seconds = Convert.ToInt32(timePassedInSeconds % 60).ToString("00");
			var minutes = (Math.Floor(timePassedInSeconds / 60) % 60).ToString("00");

			timePassed = minutes + "m " + seconds + "s";
		}

		private void GameDetailsNoSurvivors()
		{
			endGameDetails.text = TextNoSurvivors;
			endGameDetails.text += Environment.NewLine;
			endGameDetails.text += TextTimeTaken + " ";
			endGameDetails.text += timePassed;
		}

		private void GameDetailsSurvivor()
		{
			lastNpc = GameObject.FindGameObjectWithTag(Tags.Npc).GetComponentInChildren<NpcController>();

			endGameDetails.text = TextSurvivorInfo + " ";
			endGameDetails.text += lastNpc.GetHealth();
			endGameDetails.text += Environment.NewLine;
			endGameDetails.text += TextTimeTaken + " ";
			endGameDetails.text += timePassed;
		}
	}
}