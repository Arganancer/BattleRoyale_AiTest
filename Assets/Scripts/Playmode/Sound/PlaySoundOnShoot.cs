using System;
using Playmode.Event;
using Playmode.Pickable.TypePickable;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Sound
{
	public class PlaySoundOnShoot : MonoBehaviour
	{
		private ShootEventChannel shootEventChannel;
		private AudioSource[] starterWeaponShootSounds;
		private AudioSource[] uziShootSounds;
		private AudioSource shotgunShootSound;

		private void Awake()
		{
			shootEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<ShootEventChannel>();
			var aSource = GetComponents(typeof(AudioSource));
			starterWeaponShootSounds = new[]
			{
				(AudioSource) aSource[0],
				(AudioSource) aSource[1],
				(AudioSource) aSource[2]
			};
			uziShootSounds = new[]
			{
				(AudioSource) aSource[3],
				(AudioSource) aSource[4]
			};
			shotgunShootSound = (AudioSource) aSource[5];
		}

		private void OnEnable()
		{
			shootEventChannel.OnWeaponShot += PlaySound;
		}

		private void OnDisable()
		{
			shootEventChannel.OnWeaponShot -= PlaySound;
		}

		private void PlaySound(TypePickable weaponType)
		{
			switch (weaponType)
			{
				case TypePickable.Shotgun:
					shotgunShootSound.Play();
					break;
				case TypePickable.Uzi:
					uziShootSounds[CRandom.Next(0, 2)].Play();
					break;
				default:
					starterWeaponShootSounds[CRandom.Next(0, 3)].Play();
					break;
			}

		}
	}
}