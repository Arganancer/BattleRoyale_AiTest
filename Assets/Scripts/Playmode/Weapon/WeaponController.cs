using System;
using Playmode.Bullet;
using Playmode.Entity.Movement;
using UnityEngine;

namespace Playmode.Weapon
{
	public class WeaponController : MonoBehaviour
	{
		[Header("Behavior")] [SerializeField] private GameObject bulletPrefab;
		[SerializeField] private float fireDelayInSeconds = 10f;

		private float lastTimeShotInSeconds;

		private bool CanShoot => Time.time - lastTimeShotInSeconds > fireDelayInSeconds;

		private void Awake()
		{
			ValidateSerialisedFields();
			InitializeComponent();
		}

		private void ValidateSerialisedFields()
		{
			if (fireDelayInSeconds < 0)
				throw new ArgumentException("FireRate can't be lower than 0.");
		}

		private void InitializeComponent()
		{
			lastTimeShotInSeconds = 0;
		}

		public void Shoot()
		{
			if (CanShoot)
			{
				// TODO: Remove this line
				// Debug.Log("Time Fired: " + Time.time + "\nHandController position: " + transform.position);
				Instantiate(bulletPrefab, transform.position, transform.rotation);

				lastTimeShotInSeconds = Time.time;
			}
		}

		public float GetBulletSpeed()
		{
			return bulletPrefab.GetComponentInChildren<Mover>().GetSpeed();
		}
	}
}