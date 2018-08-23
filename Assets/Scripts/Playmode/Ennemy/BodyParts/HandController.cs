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
            //TODO : Utilisez ce que vous savez des vecteurs pour implémenter cette méthode
            throw new NotImplementedException();
        }

        public void Use()
        {
            if (weapon != null) weapon.Shoot();
        }
    }
}