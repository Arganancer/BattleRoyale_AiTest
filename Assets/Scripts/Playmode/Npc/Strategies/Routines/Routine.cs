using Playmode.Entity.Movement;

namespace Playmode.Npc.Strategies.Routines
{
	public abstract class Routine
	{
		protected const float MinRoutineTime = 0.3f;
		protected const float MaxRoutineTime = 0.7f;
		
		protected readonly Mover Mover;
		
		protected float SubRoutineTimeRemaining; //BEN_CORRECTION : Ça veut dire quoi ça ?
		
		protected Routine(Mover mover)
		{
			Mover = mover;
			SubRoutineTimeRemaining = 0f;
		}
		
		protected abstract void UpdateSubRoutine();
	}
}