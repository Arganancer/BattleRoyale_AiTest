using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Npc.Strategies.Routines.SightRoutines;

namespace Playmode.Npc.Strategies
{
	public class NormalBehavior : BaseNpcBehavior
	{
		private readonly SightRoutine noEnemySightRoutine;

		public NormalBehavior(Mover mover, HandController handController,Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController,
			health, npcSensorSight, npcSensorSound)
		{
			HealthRetreatTolerance = 0;
			noEnemySightRoutine = new LookAroundSightRoutine(Mover);
			DistanceSwitchFromAttackingToEngaging = 6f;
			DistanceSwitchFromEngagingToAttacking = 5f;
		}

		protected override void DoIdle()
		{
			Mover.RotateTowardsAngle(RotationOrientation);
		}

		protected override void DoRoaming()
		{
			if (IsOutsideOfZone)
				//BEN_REVIEW : Vous exploitez le fait qu'une position soit un vecteur, en plus du fait que le mode est
				//			   toujours centré au point (0,0). Vous pourriez avoir un méchant bogue éventuellement si
			    //			   vous faites des changements sur la structure de vos scènes.
				MovementDirection = -Mover.transform.parent.root.position;
			
			Mover.MoveTowardsDirection(MovementDirection);
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}

		protected override void DoEngaging()
		{
			//BEN_REVIEW : Ce que je trouve étrange parfois dans votre code, c'est que l'information est parfois complète
			//			   dès l'entrée dans un état, et d'autres fois non. C'est comme si votre behavior entrait dans un état
			//			   invalide pour se corriger par la suite. Règle générale, un objet devrait toujours rester dans
			//			   un état valide, sauf s'il considère qu'il peut être détruit.
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = NpcSensorSight.GetClosestNpc();

			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			Mover.MoveTowardsPosition(CurrentEnemyTarget.transform.root.position);

			HandController.Use();
		}

		protected override void DoInvestigating()
		{
			MovementDirection = NpcSensorSound.GetNewestSoundPosition() - Mover.transform.root.position;
			
			Mover.MoveTowardsDirection(MovementDirection);
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = NpcSensorSight.GetClosestNpc();
			
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			
			HandController.Use();
		}

		protected override void DoRetreating()
		{
			//BEN_CORRECTION : Exactement ce que je disais dans BaseNpcBehavior.
			// Too dumb to retreat
		}

		protected override State EvaluateIdle()
		{
			if (NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;

			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateIdle();
		}

		protected override State EvaluateRoaming()
		{
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

			return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
				return State.Idle;

			return DistanceToCurrentEnemy > DistanceSwitchFromAttackingToEngaging ? State.Engaging : State.Attacking;
		}

		protected override State EvaluateRetreating()
		{
			return State.Idle;
		}
	}
}