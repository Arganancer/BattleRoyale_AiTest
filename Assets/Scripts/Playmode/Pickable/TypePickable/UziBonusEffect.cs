using Playmode.Npc;

namespace Playmode.Pickable.TypePickable
{
	public class UziBonusEffect : PickableBonusEffect 
	{
		public override void ApplyBonusEffect(NpcController npcController)
		{
			npcController.PickUzi();
		}
	}
}

