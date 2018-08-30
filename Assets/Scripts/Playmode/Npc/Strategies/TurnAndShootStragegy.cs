using Playmode.Entity.Movement;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;

namespace Playmode.Npc.Strategies
{
    public class TurnAndShootStragegy : INpcStrategy
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

        public void ReactToEnemyInSight(NpcController npc)
        {
            
        }
        
        public void ReactToLooseOfEnemySight(NpcController enemy)
        {
            
        }
    }
}