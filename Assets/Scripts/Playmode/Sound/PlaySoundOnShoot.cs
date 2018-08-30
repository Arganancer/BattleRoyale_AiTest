﻿using Playmode.Event;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Sound
{
	public class PlaySoundOnShoot : MonoBehaviour
	{
		private ShootEventChannel shootEventChannel;
		private AudioSource audioSource;

		private void Awake()
		{
			shootEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<ShootEventChannel>();
			var aSource = GetComponents(typeof(AudioSource));
			audioSource = (AudioSource) aSource[0];
		}

		private void OnEnable()
		{
			shootEventChannel.OnEventPublished += PlaySound;
		}

		private void OnDisable()
		{
			shootEventChannel.OnEventPublished -= PlaySound;
		}

		private void PlaySound()
		{
			audioSource.Play();
		}
	}
}