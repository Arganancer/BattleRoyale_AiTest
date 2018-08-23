namespace Playmode.Entity.Destruction
{
    public class RootDestroyer : Destroyer
    {
        public override void Destroy()
        {
            Destroy(transform.root.gameObject);
        }
    }
}