using Playmode.Ennemy.BodyParts;
using Playmode.Entity.Movement;

namespace Playmode.Ennemy.Strategies
{
    public class TurnAndShootStragegy : IEnnemyStrategy
    {
        private readonly Mover mover;
        private readonly HandController handController;

        public TurnAndShootStragegy(Mover mover, HandController handController)
        {
            this.mover = mover;
            this.handController = handController;
        }

        public void Act()
        {
            mover.Rotate(Mover.Clockwise);

            handController.Use();
        }

        public void ReactToEnemyInSight(EnnemyController ennemy)
        {
            
        }
        
        public void ReactToLooseOfEnemySight(EnnemyController enemy)
        {
            
        }
    }
}