namespace Playmode.Ennemy.Strategies
{
    public interface IEnnemyStrategy
    {
        void Act();
    }

    public enum EnnemyStrategy
    {
        Normal,
        Careful,
        Cowboy,
        Camper
    }
}