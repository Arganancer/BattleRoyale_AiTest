using Playmode.Entity.Movement;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Npc.Strategies.Routines.SightRoutines
{
	public class LookAroundSightRoutine : SightRoutine
	{
		public LookAroundSightRoutine(Mover mover) : base(mover)
		{
		}

		protected override void UpdateSubRoutine()
		{
			if (CurrentSightRoutineDelay > 0f)
			{
				CurrentSightRoutineDelay -= Time.deltaTime;
			}
			else if (CurrenSightSubRoutineType == SightSubRoutineType.LookForward)
			{
				var nextSightRoutine = CRandom.Next(1, 3);
				CurrenSightSubRoutineType =
					nextSightRoutine <= 1 ? SightSubRoutineType.LookLeft : SightSubRoutineType.LookRight;
			}
		}
	}
}