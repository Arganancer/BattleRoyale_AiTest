using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Npc.Strategies.Routines.MovementRoutines;
using Playmode.Npc.Strategies.Routines.SightRoutines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Npc.Strategies
{
	/// <summary>
	/// Prudent. Dès qu’il trouve un ennemi, il tire dessus en gardant ses distances.
	/// Si sa vie est trop basse, il tente immédiatement de trouver un « MedicalKit ».
	/// S’il croise une arme, il se dirige dessus que s’il n’est pas à la recherche d’un « MedicalKit ».
	/// </summary>
	public class CarefulBehavior : BaseNpcBehavior
	{
		private readonly SightRoutine noEnemySightRoutine;
		private float distanceSwitchFromAttackingToRetreating = 20f;

		public CarefulBehavior(Mover mover, HandController handController, Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController,
			health, npcSensorSight, npcSensorSound)
		{
			noEnemySightRoutine = new LookAroundSightRoutine(Mover);

			HealthRetreatTolerance = 800;
			DistanceSwitchFromAttackingToEngaging = 22f;
			DistanceSwitchFromEngagingToAttacking = 21f;
		}

		protected override void DoIdle()
		{
			Mover.RotateTowardsAngle(RotationOrientation);
		}

		protected override void DoRoaming()
		{
			Mover.MoveTowardsDirection(MovementDirection);
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}

		protected override void DoInvestigating()
		{
			if (Health.HealthPoints < HealthRetreatTolerance)
				MovementDirection = -(NpcSensorSound.GetNewestSoundPosition() - Mover.transform.root.position);
			else
				MovementDirection = NpcSensorSound.GetNewestSoundPosition() - Mover.transform.root.position;
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
			Mover.MoveTowardsDirection(MovementDirection);
		}

		protected override void DoEngaging()
		{
			if (Health.HealthPoints < HealthRetreatTolerance &&
			    CurrentMedicalKitTarget != null)
			{
				Mover.RotateTowardsPosition(CurrentMedicalKitTarget.transform.root.position);
				Mover.MoveTowardsPosition(CurrentMedicalKitTarget.transform.root.position);
			}
			else if (CurrentEnemyTarget != null)
			{
				Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
				Mover.MoveTowardsPosition(CurrentEnemyTarget.transform.root.position);
				HandController.Use();
			}
			else if (CurrentUziTarget != null)
			{
				Mover.RotateTowardsPosition(CurrentUziTarget.transform.root.position);
				Mover.MoveTowardsPosition(CurrentUziTarget.transform.root.position);
			}
		}

		protected override void DoAttacking()
		{
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			HandController.Use();
		}

		protected override void DoRetreating()
		{
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			Mover.MoveAwayFromPosition(CurrentEnemyTarget.transform.root.position);
			HandController.Use();
		}

		protected override State EvaluateIdle()
		{
			if (CurrentMedicalKitTarget != null && Health.HealthPoints < HealthRetreatTolerance)
				return State.Engaging;
			if (NpcSensorSight.NpcsInSight.Any() || CurrentUziTarget != null)
				return State.Engaging;

			return NpcSensorSound.SoundsInformations.Any()
				? State.Investigating
				: base.EvaluateIdle();
		}

		protected override State EvaluateRoaming()
		{
			if (CurrentMedicalKitTarget != null && Health.HealthPoints < HealthRetreatTolerance)
				return State.Engaging;
			if (NpcSensorSight.NpcsInSight.Any() || CurrentUziTarget != null)
				return State.Engaging;

			return NpcSensorSound.SoundsInformations.Any()
				? State.Investigating
				: base.EvaluateRoaming();
		}

		protected override State EvaluateInvestigating()
		{
			if (CurrentMedicalKitTarget != null && Health.HealthPoints < HealthRetreatTolerance)
				return State.Engaging;
			if (NpcSensorSight.NpcsInSight.Any() || CurrentUziTarget != null)
				return State.Engaging;

			return !NpcSensorSound.SoundsInformations.Any() ? State.Idle : State.Investigating;
		}

		protected override State EvaluateEngaging()
		{
			if (NpcSensorSight.NpcsInSight.Any())
			{
				if (Health.HealthPoints < HealthRetreatTolerance)
					return CurrentMedicalKitTarget != null ? State.Engaging : State.Retreating;
				return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
			}
			
			if (Health.HealthPoints >= HealthRetreatTolerance && CurrentUziTarget != null)
				return State.Engaging;
			
			if (Health.HealthPoints < HealthRetreatTolerance && CurrentMedicalKitTarget != null)
				return State.Engaging;

			return State.Idle;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
				return State.Idle;

			if (Health.HealthPoints < HealthRetreatTolerance ||
			    DistanceToCurrentEnemy < distanceSwitchFromAttackingToRetreating)
				return State.Retreating;

			return DistanceToCurrentEnemy > DistanceSwitchFromAttackingToEngaging ? State.Engaging : State.Attacking;
		}

		protected override State EvaluateRetreating()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
				return State.Idle;
			
			if (Health.HealthPoints >= HealthRetreatTolerance &&
			    DistanceToCurrentEnemy > DistanceSwitchFromEngagingToAttacking)
				return State.Attacking;

			return CurrentMedicalKitTarget != null ? State.Engaging : State.Retreating;
		}
	}
}