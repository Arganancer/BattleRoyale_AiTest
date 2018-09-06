using Playmode.Npc;

namespace Playmode.Pickable.TypePickable
{
	public class ShotgunBonusEffect : PickableBonusEffect {
		public override void ApplyBonusEffect(NpcController npcController)
		{
			npcController.PickShotgun();
		}
	}
}

