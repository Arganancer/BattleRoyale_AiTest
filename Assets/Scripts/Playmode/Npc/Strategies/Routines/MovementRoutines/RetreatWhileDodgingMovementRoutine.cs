using Playmode.Entity.Movement;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Npc.Strategies.Routines.MovementRoutines
{
	public class RetreatWhileDodgingMovementRoutine : MovementRoutine
	{
		public RetreatWhileDodgingMovementRoutine(Mover mover) : 
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
				SubRoutineTimeRemaining = CRandom.Nextf(MinRoutineTime, MaxRoutineTime);
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
					CurrenMovementSubRoutineType = MovementSubRoutineType.MovingBackward;;
				}
			}
		}
	}
}