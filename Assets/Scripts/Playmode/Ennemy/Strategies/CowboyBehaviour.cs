using System;
using System.Collections.Generic;
using Playmode.Ennemy.BodyParts;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using UnityEngine;

using Playmode.Entity.Senses;

namespace Playmode.Ennemy.Strategies
{
    //[RequireComponent(typeof(EnnemyController))] 
    
    public class CowboyBehaviour : IEnnemyStrategy
    {
        private readonly Mover mover;
        private readonly HandController handController;
        private EnnemyController ennemyController;
        private GameObject gameObject;
        private EnnemySensor ennemySensor;

        private bool hasEnemyTarget;
        private bool hasMunitionTarget;
        
        public CowboyBehaviour(Mover mover, HandController handController)
        {
            this.mover = mover;
            this.handController = handController;
        }
        
        public void Act()
        {
            var position = mover.transform.position;
            
            if (hasEnemyTarget)
            {
                ennemySensor.See(ennemyController);
                mover.MoveRelativeToSelf(ennemyController.transform.position - position);
            }
            else if (hasMunitionTarget)
            {
                mover.MoveRelativeToSelf(gameObject.transform.position - position);
            }
        }

        public void ReactToEnemyInSight(EnnemyController enemy)
        {
            throw new System.NotImplementedException();
        }

        public void ReactToLooseOfEnemySight(EnnemyController enemy)
        {
            throw new System.NotImplementedException();
        }
    }
}