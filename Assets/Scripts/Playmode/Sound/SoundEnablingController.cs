using UnityEngine;

namespace Playmode.Sound
{
	//BEN_CORRECTION : Inutilisé. Et j'en vois pas l'utilité non plus.
	public class SoundEnablingController : MonoBehaviour
	{
		public void PlayAndStopAudio()
		{
			AudioListener.pause = !AudioListener.pause;
		}
	}
}