using Playmode.Npc;

namespace Playmode.Pickable.TypePickable
{
	public class MedkitBonusEffect : PickableBonusEffect 
	{
		public override void ApplyBonusEffect(NpcController npcController)
		{
			npcController.Heal(500);
		}
	}
}

