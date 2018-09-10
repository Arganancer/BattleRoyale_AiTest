using Playmode.Npc;
using Playmode.Pickable;
using UnityEngine;

namespace Playmode.World
{
	//BEN_CORRECTION : Pas un véritable sensor. Où sont les événements ? Directement dépendant de NPCController, ce
	//				   qui ne devrait pas être le cas.
	//
	//				   En plus, dépend de PickableController. Autrement dit, ce composant est "zéro" réutilisable.
	
	public class ZoneSensor : MonoBehaviour
	{
		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.transform.root?.GetComponentInChildren<NpcController>())
			{
				NotifyObjectLeavingZone(other.gameObject.transform.root
					.GetComponentInChildren<NpcController>());
			}
			else if (other.gameObject.transform.root?.GetComponentInChildren<PickableController>())
			{
				DestroyOutOfZonePickable(other.gameObject.transform.root
					.GetComponentInChildren<PickableController>());
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.transform.root?.GetComponentInChildren<NpcController>())
			{
				NotifyObjectEnteringZone(other.gameObject.transform.root
					.GetComponentInChildren<NpcController>());
			}
		}

		private static void NotifyObjectLeavingZone(NpcController npcController)
		{
			npcController.UpdateNpcStateExitZone();
		}

		private static void NotifyObjectEnteringZone(NpcController npcController)
		{
			npcController.UpdateNpcStateEnterZone();
		}

		private static void DestroyOutOfZonePickable(PickableController pickableController)
		{
			pickableController.DestroyPickable();
		}
	}
}

//BEN_REVIEW : Oooofff.....ENFIN, fini de corriger votre code!!! J'ai jamais vu quelque chose d'aussi compliqué pour
//			   si peu.
