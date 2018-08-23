using UnityEngine;

namespace Playmode.Entity.Senses
{
    public delegate void HitSensorEventHandler(int hitPoints);

    public class HitSensor : MonoBehaviour
    {
        public event HitSensorEventHandler OnHit;

        public void Hit(int hitPoints)
        {
            NotifyHit(hitPoints);
        }

        private void NotifyHit(int hitPoints)
        {
            if (OnHit != null) OnHit(hitPoints);
        }
    }
}