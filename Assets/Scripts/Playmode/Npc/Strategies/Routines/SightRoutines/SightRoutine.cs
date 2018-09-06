using System;
using Playmode.Entity.Movement;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Npc.Strategies.Routines.SightRoutines
{
	public abstract class SightRoutine : Routine
	{
		private const float MinSightRoutineDelay = 0.2f;
		private const float MaxSightRoutineDelay = 1.1f;
		private const float LookAngleTolerance = 70f;
		
		protected SightSubRoutineType CurrenSightSubRoutineType;
		
		protected float CurrentSightRoutineDelay;
		
		protected SightRoutine(Mover mover) : base(mover)
		{
			CurrenSightSubRoutineType = SightSubRoutineType.LookForward;
		}
		
		public void UpdateSightRoutine(Vector3 movementDirection)
		{
			UpdateSubRoutine();
			DoSightSubRoutine(movementDirection);
		}

		private void DoSightSubRoutine(Vector3 movementDirection)
		{
			switch (CurrenSightSubRoutineType)
			{
				case SightSubRoutineType.LookLeft:
					LookToTheLeft(movementDirection);
					break;
				case SightSubRoutineType.LookRight:
					LookToTheRight(movementDirection);
					break;
				case SightSubRoutineType.LookForward:
					Mover.RotateTowardsDirection(movementDirection);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private void StartSightRoutineDelay()
		{
			CurrentSightRoutineDelay = CRandom.Nextf(MinSightRoutineDelay, MaxSightRoutineDelay);
			CurrenSightSubRoutineType = SightSubRoutineType.LookForward;
		}

		private void LookToTheRight(Vector3 movementDirection)
		{
			if (Vector3.Angle(Mover.transform.up, movementDirection) < LookAngleTolerance)
			{
				Mover.Rotate(1);
			}
			else
			{
				StartSightRoutineDelay();
			}
		}

		private void LookToTheLeft(Vector3 movementDirection)
		{
			if (Vector3.Angle(Mover.transform.up, movementDirection) < LookAngleTolerance)
			{
				Mover.Rotate(-1);
			}
			else
			{
				StartSightRoutineDelay();
			}
		}
	}
}