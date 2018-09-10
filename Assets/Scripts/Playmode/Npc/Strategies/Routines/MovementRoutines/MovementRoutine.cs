using System;
using Playmode.Entity.Movement;
using UnityEngine;

namespace Playmode.Npc.Strategies.Routines.MovementRoutines
{
	//BEN_CORRECTION : C'est une classe abstraite, je m'attendais donc à du polymorphisme, mais non, j'ai eu
	//				   un "Switch Case".
	//
	//				   Une classe ne devrait jamais dépendre de ses enfants. Pourtant, c'est le cas ici.
	public abstract class MovementRoutine : Routine
	{
		protected MovementSubRoutineType CurrenMovementSubRoutineType;

		protected MovementRoutine(Mover mover) : base(mover)
		{
			CurrenMovementSubRoutineType = MovementSubRoutineType.MovingBackward;
		}

		public void UpdateMovementRoutine(Vector3 target)
		{
			UpdateSubRoutine();
			DoMovementSubRoutine(target);
		}

		private void DoMovementSubRoutine(Vector3 target)
		{
			switch (CurrenMovementSubRoutineType)
			{
				case MovementSubRoutineType.MovingBackward:
					Mover.MoveAwayFromPosition(target);
					break;
				case MovementSubRoutineType.MovingRight:
					Mover.MoveRightAroundPosition(target);
					break;
				case MovementSubRoutineType.MovingLeft:
					Mover.MoveLeftAroundPosition(target);
					break;
				case MovementSubRoutineType.MovingForward:
					Mover.MoveTowardsPosition(target);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}