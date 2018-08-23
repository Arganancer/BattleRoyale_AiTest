using System;
using UnityEngine;

namespace Playmode.Movement
{
    public abstract class Mover : MonoBehaviour
    {
        public static readonly Vector3 Foward = Vector3.up;
        public const float Clockwise = 1f;
        
        [SerializeField] protected float speed = 1f;
        [SerializeField] protected float rotateSpeed = 90f;

        protected void Awake()
        {
            ValidateSerialisedFields();
        }

        private void ValidateSerialisedFields()
        {
            if (speed < 0)
                throw new ArgumentException("Speed can't be lower than 0.");
            if (rotateSpeed < 0)
                throw new ArgumentException("RotateSpeed can't be lower than 0.");
        }

        public abstract void Move(Vector3 direction);

        public abstract void Rotate(float direction);
    }
}