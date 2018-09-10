using Playmode.Event;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Sound
{
	//BEN_REVIEW : N'aurait-il pas été plus simple de mettre un "AudioSource" sur chaque NPC et de faire en sorte que chaque
	//			   NPC émette son propre son ?
	//
	//			   Aussi, j'ai trouvé une quantité "astronomique" de "AudioSource" dans le GameObject "GameController". Un
	//			   seul aurait suffit. Ensuite, il suffit d'utiliser "audioSource.PlayOneShot(unAudioClip)".
	//
	//			   Voir ceci : https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
	//
	//			   You're welcome.
	public class PlaySoundOnHit : MonoBehaviour
	{
		private HitEventChannel hitEventChannel;
		private AudioSource audioSource;

		private void Awake()
		{
			hitEventChannel = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<HitEventChannel>();
			
			var aSources = GetComponents(typeof(AudioSource));
			audioSource = (AudioSource) aSources[7];
		}

		private void OnEnable()
		{
			hitEventChannel.OnEventPublished += PlaySound;
		}

		private void OnDisable()
		{
			hitEventChannel.OnEventPublished -= PlaySound;
		}

		private void PlaySound()
		{
			audioSource.Play();
		}
	}
}