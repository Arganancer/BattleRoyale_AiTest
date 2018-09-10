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
		//BEN_CORRECTION : Devrait être en "SerializedFields" pour être modifiables dans l'éditeur.
		//				   Tout ce qui est "Game Data" devrait être modifiable dans l'éditeur (vitesse des ennemis,
		//				   textes, etc...)
		private const string TextNoSurvivors = "No survivors !";
		private const string TextSurvivorInfo = "Survivor's remaining HP : ";
		private const string TextTimeTaken = "Time taken :";
		private const float RegularTimeSpeed = 1.0f;
		private const float PausedTimeSpeed = 0.0f;

		private NpcDeathEventChannel npcDeathEventChannel;
		private GameObject pauseObjects;
		private GameObject endGameObjects;
		private NpcController lastNpc;
		private Text endGameDetails; //BEN_CORRECTION : UI ne devrait pas être géré par le GameController.

		private int numberOfNpcs;
		private bool isGamePaused;
		private float timePassedInSeconds;
		private string timePassed; //BEN_CORRECTION : Il n'y a aucune raison pour laquelle cet attribut n'est pas juste une variable. Overdesign.

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
			
			//BEN_CORRECTION : Je peux comprendre que le GameController puisse mettre le jeu en pause, mais je ne vois
			//				   pourquoi c'est sa responsabilité d'afficher le menu. Le menu devrait s'afficher de lui même
			//				   lorsque le jeu devient en pause.
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

			//BEN_REVIEW : Utilisez "string.format" SVP.
			timePassed = minutes + "m " + seconds + "s";
		}

		private void GameDetailsNoSurvivors()
		{
			//BEN_REVIEW : Utilisez "string.format" SVP.
			endGameDetails.text = TextNoSurvivors;
			endGameDetails.text += Environment.NewLine;
			endGameDetails.text += TextTimeTaken + " ";
			endGameDetails.text += timePassed;
		}

		private void GameDetailsSurvivor()
		{
			lastNpc = GameObject.FindGameObjectWithTag(Tags.Npc).GetComponentInChildren<NpcController>();

			//BEN_REVIEW : Utilisez "string.format" SVP.
			endGameDetails.text = TextSurvivorInfo + " ";
			endGameDetails.text += lastNpc.GetHealth();
			endGameDetails.text += Environment.NewLine;
			endGameDetails.text += TextTimeTaken + " ";
			endGameDetails.text += timePassed;
		}
	}
}