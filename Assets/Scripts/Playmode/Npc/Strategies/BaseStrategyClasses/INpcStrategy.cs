namespace Playmode.Npc.Strategies.BaseStrategyClasses
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