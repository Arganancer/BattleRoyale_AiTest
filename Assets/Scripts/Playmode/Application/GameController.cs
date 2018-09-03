﻿using Playmode.Event;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Application
{
	public class GameController : MonoBehaviour
	{
		private MainController mainController;
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
			npcDeathEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<NpcDeathEventChannel>();

			isGamePaused = false;
		}

		private void OnEnable()
		{
			npcDeathEventChannel.OnEventPublished += DecrementNumberOfNpcs;
		}

		private void Update()
		{
			/*if (IsGameOver)
			{
				GameObject.FindGameObjectWithTag(Tags.MainController).GetComponent<MainController>().StopGame();
			}*/

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
			Time.timeScale = 0;
		}

		private void UnpauseGame()
		{
			Time.timeScale = 1;
		}
	}
}