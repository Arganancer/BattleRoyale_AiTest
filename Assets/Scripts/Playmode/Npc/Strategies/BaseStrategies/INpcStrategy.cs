namespace Playmode.Npc.Strategies
{
	public interface INpcStrategy
	{
		void Act();
	}

	public enum NpcStrategy
	{
		Normal,
		Careful,
		Cowboy,
		Camper
	}
}