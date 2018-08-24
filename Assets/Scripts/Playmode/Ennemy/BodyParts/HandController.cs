using System;
using Playmode.Movement;
using Playmode.Weapon;
using UnityEngine;

namespace Playmode.Ennemy.BodyParts
{
    public class HandController : MonoBehaviour
    {
        private Mover mover;
        private WeaponController weapon;

        private void Awake()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            mover = GetComponent<AnchoredMover>();
        }
        
        public void Hold(GameObject gameObject)
        {
            if (gameObject != null)
            {
                gameObject.transform.parent = transform;
                gameObject.transform.localPosition = Vector3.zero;
                
                weapon = gameObject.GetComponentInChildren<WeaponController>();
            }
            else
            {
                weapon = null;
            }
        }

        public void AimTowards(GameObject target)
        {
            Vector2 direction = GetDirectionTowardTheEnemy(target);
            float angle = (float)GetAngleOfTheEnemyDirection(direction);
            SetActorDirection(angle);
        }

        public void Use()
        {
            if (weapon != null) weapon.Shoot();
        }

        private Vector2 GetDirectionTowardTheEnemy(GameObject target)
        {
            return target.transform.position - mover.transform.position;
        }
        
        public void SetActorDirection(float angle)
        {
            mover.Rotate(-angle);
        }

        private double GetAngleOfTheEnemyDirection(Vector2 direction)
        {
            double angleInRadian = Math.Atan2(direction.y, direction.x);
            return (angleInRadian / Math.PI) * 180;
        }
    }
}