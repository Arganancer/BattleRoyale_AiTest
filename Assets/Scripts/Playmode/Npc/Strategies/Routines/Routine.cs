using Playmode.Entity.Movement;
using UnityEngine;

namespace Playmode.Npc.Strategies.Routines
{
	public abstract class Routine
	{
		protected float SubRoutineTimeRemaining;
		protected readonly Mover Mover;
		protected const float MinRoutineTime = 0.3f;
		protected const float MaxRoutineTime = 0.7f;

		protected Routine(Mover mover)
		{
			Mover = mover;
			SubRoutineTimeRemaining = 0f;
		}
		
		protected abstract void UpdateSubRoutine();
	}
}