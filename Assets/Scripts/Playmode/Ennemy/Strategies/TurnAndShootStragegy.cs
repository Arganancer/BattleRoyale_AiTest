using Playmode.Ennemy.BodyParts;
using Playmode.Movement;

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
    }
}