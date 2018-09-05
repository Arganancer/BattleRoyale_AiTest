using UnityEngine;

namespace Playmode.Sound
{
	public class SoundEnablingController : MonoBehaviour
	{
		public void PlayAndStopAudio()
		{
			AudioListener.pause = !AudioListener.pause;
		}
	}
}