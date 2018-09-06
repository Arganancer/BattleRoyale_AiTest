using Playmode.Npc;

namespace Playmode.Pickable.TypePickable
{
	public class MedkitBonusEffect : PickableBonusEffect
	{
		private const int HealthBonus = 500;
		
		public override void ApplyBonusEffect(NpcController npcController)
		{
			npcController.Heal(HealthBonus);
		}
	}
}

