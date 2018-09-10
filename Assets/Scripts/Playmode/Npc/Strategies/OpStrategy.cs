using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Npc.Strategies.Routines.MovementRoutines;
using Playmode.Npc.Strategies.Routines.SightRoutines;

namespace Playmode.Npc.Strategies
{
	public class OpStrategy : BaseNpcBehavior
	{
		private readonly MovementRoutine retreatingMovementRoutine;
		private readonly MovementRoutine engagingMovementRoutine;
		private readonly SightRoutine noEnemySightRoutine;
		
		public OpStrategy(Mover mover, HandController handController, Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController,
			health, npcSensorSight, npcSensorSound)
		{
			HealthRetreatTolerance = 400;
			retreatingMovementRoutine = new RetreatWhileDodgingMovementRoutine(Mover);
			engagingMovementRoutine = new AdvanceWhileDodgingMovementRoutine(Mover);
			noEnemySightRoutine = new LookAroundSightRoutine(Mover);
		}

		//BEN_REVIEW : Soudainement, des #region. Standardisez votre code svp.
		
		#region DoTheStuff

		protected override void DoIdle()
		{
			Mover.Rotate(RotationOrientation);
		}

		protected override void DoRoaming()
		{
			Mover.MoveTowardsDirection(MovementDirection);
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}
		
		protected override void DoInvestigating()
		{
			MovementDirection = NpcSensorSound.GetNewestSoundPosition() - Mover.transform.root.position;
			
			Mover.MoveTowardsDirection(MovementDirection);
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);

		}

		protected override void DoEngaging()
		{
			engagingMovementRoutine.UpdateMovementRoutine(CurrentEnemyTarget.transform.root.position);
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));

			HandController.Use();
		}

		protected override void DoAttacking()
		{
			Mover.MoveRightAroundPosition(CurrentEnemyTarget.transform.root.position);
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));

			HandController.Use();
		}

		protected override void DoRetreating()
		{
			if(!IsOutsideOfZone)
				retreatingMovementRoutine.UpdateMovementRoutine(NpcSensorSight.GetClosestNpc().transform.root.position);
			
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			
			HandController.Use();
		}

		#endregion

		#region Evaluate

		protected override State EvaluateIdle()
		{	
			if (NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;
			
			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateIdle();
		}

		protected override State EvaluateRoaming()
		{	
			if (IsOutsideOfZone)
				MovementDirection = -Mover.transform.parent.root.position;
			
			if (NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;

			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateRoaming();
		}

		protected override State EvaluateInvestigating()
		{
			if (NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;
			
			return !NpcSensorSound.SoundsInformations.Any() ? State.Idle : State.Investigating;
		}

		protected override State EvaluateEngaging()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
				return State.Idle;

			if (Health.HealthPoints < 50)
				return State.Retreating;

			return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
				return State.Idle;

			if (Health.HealthPoints < 80)
				return State.Retreating;

			return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateRetreating()
		{
			if (NpcSensorSight.NpcsInSight.Any()) return State.Retreating;
			
			TimeUntilStateSwitch = MaxRoamingTime;
			
			return State.Roaming;
		}

		#endregion
	}
}