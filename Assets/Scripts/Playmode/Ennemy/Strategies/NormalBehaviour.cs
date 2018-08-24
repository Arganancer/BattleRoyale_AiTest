using System;
using System.Collections;
using System.Collections.Generic;
using Playmode.Ennemy.BodyParts;
using Playmode.Ennemy.Strategies;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using UnityEngine;

namespace Playmode.Ennemy.Strategies
{
    public class NormalBehaviour : IEnnemyStrategy
    {
        private bool hasTarget;
        private readonly Mover mover;
        private readonly HandController handController;
        private EnnemyController ennemyController;
        private EnnemySensor ennemySensor;
        public NormalBehaviour(Mover mover, HandController handController)
        {
            this.mover = mover;
            this.handController = handController;
            ennemyController = null;
        }

        public void Act()
        {
            Vector2 direction = GetDirectionTowardTheEnemy();
            float angle = (float)GetAngleOfTheEnemyDirection(direction);
            SetActorDirection(angle);
            MoveAndShootTowardTheEnemy(direction);
        }

        public void ReactToEnemyInSight(EnnemyController ennemy)
        {
            if (ennemyController == null)
            {
                ennemyController = ennemy;
            }
        }

        public void SetActorDirection(float angle)
        {
            mover.Rotate(-angle);
        }
        public void MoveAndShootTowardTheEnemy(Vector2 direction)
        {
            mover.MoveRelativeToSelf(direction*Time.deltaTime);
            handController.Use();
        }

        private double GetAngleOfTheEnemyDirection(Vector2 direction)
        {
            double angleInRadian = Math.Atan2(direction.y, direction.x);
            return (angleInRadian / Math.PI) * 180;
        }
        private Vector2 GetDirectionTowardTheEnemy()
        {
            return ennemyController.transform.position - mover.transform.position;
        }

        public void ReactToLooseOfEnemySight(EnnemyController enemy)
        {
            if (enemy == ennemyController)
            {
                ennemyController = null;
            }
        }
    }
}

