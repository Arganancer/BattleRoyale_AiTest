using Playmode.Entity.Movement;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Npc.Strategies.Routines.MovementRoutines
{
	public class AdvanceWhileDodgingMovementRoutine : MovementRoutine
	{
		public AdvanceWhileDodgingMovementRoutine(Mover mover) : 
			base(mover)
		{
		}

		protected override void UpdateSubRoutine()
		{
			if (SubRoutineTimeRemaining > 0f)
			{
				SubRoutineTimeRemaining -= Time.deltaTime;
			}
			else
			{
				var chanceOfRetreatingRoutine = CRandom.Next(1, 4);
				SubRoutineTimeRemaining = CRandom.Nextf(0.3f, 0.7f);
				
				if (chanceOfRetreatingRoutine <= 1)
				{
					CurrenMovementSubRoutineType = MovementSubRoutineType.MovingLeft;
				}
				else if (chanceOfRetreatingRoutine <= 2)
				{
					CurrenMovementSubRoutineType = MovementSubRoutineType.MovingRight;
				}
				else
				{
					CurrenMovementSubRoutineType = MovementSubRoutineType.MovingForward;;
				}
			}
		}
	}
}