using System;
using UnityEngine;

namespace Playmode.Entity.Senses
{
    public class HitStimulus : MonoBehaviour
    {
        [Header("Behaviour")] [SerializeField] private int hitPoints = 10;

        private void Awake()
        {
            ValidateSerializeFields();
        }

        private void ValidateSerializeFields()
        {
            if (hitPoints < 0)
                throw new ArgumentException("Hit points can't be less than 0.");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            other.GetComponent<Entity.Senses.HitSensor>()?.Hit(hitPoints);
        }
    }
}