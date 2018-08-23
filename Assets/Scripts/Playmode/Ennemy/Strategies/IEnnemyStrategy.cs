namespace Playmode.Ennemy.Strategies
{
    public interface IEnnemyStrategy
    {
        void Act();
        void ReactToEnemyInSight(EnnemyController enemy);
        void ReactToLooseOfEnemySight(EnnemyController enemy);
    }

    public enum EnnemyStrategy
    {
        Normal,
        Careful,
        Cowboy,
        Camper
    }
}