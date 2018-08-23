using System;
using UnityEngine;

namespace Playmode.Entity.Status
{
    public delegate void HealthEventHandler();

    public class Health : MonoBehaviour
    {
        [SerializeField] private int healthPoints = 100;

        public event HealthEventHandler OnDeath;

        public int HealthPoints
        {
            get { return healthPoints; }
            private set
            {
                healthPoints = value < 0 ? 0 : value;

                if (healthPoints <= 0) NotifyDeath();
            }
        }

        private void Awake()
        {
            ValidateSerialisedFields();
        }

        private void ValidateSerialisedFields()
        {
            if (healthPoints < 0)
                throw new ArgumentException("HealthPoints can't be lower than 0.");
        }

        public void Hit(int hitPoints)
        {
            HealthPoints -= hitPoints;
        }

        private void NotifyDeath()
        {
            if (OnDeath != null) OnDeath();
        }
    }
}