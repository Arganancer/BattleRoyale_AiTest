using Playmode.Npc;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public class NpcSoundStimulus : MonoBehaviour
	{
		private NpcController npc;

		private void Awake()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			npc = transform.root.GetComponentInChildren<NpcController>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			other.GetComponent<NpcSensorSound>()?.EnterSoundRange(npc);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			other.GetComponent<NpcSensorSound>()?.LeaveSoundRange(npc);
		}
	}
}