using UnityEngine;

namespace Playmode.Sound
{
	public class SoundManager : MonoBehaviour
	{
		public static SoundManager instance = null;
		
		public AudioSource shotSound;
		public AudioSource deathSound;

		private void Awake()
		{
			if (instance == null)
				instance = this;
			else if (instance != this)
				Destroy(gameObject);
			
			DontDestroyOnLoad(gameObject);
		}

		public void PlaySound(AudioClip clip)
		{
			
		}
	}
}