using System;
using Playmode.Bullet;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Event;
using Playmode.Pickable.TypePickable;
using Playmode.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Weapon
{
	public class WeaponController : MonoBehaviour
	{
		[Header("Behavior")] [SerializeField] private GameObject bulletPrefab;
		
		[SerializeField] private float fireDelayInSeconds = 0.5f;
		[SerializeField] private int nbOfShotgunBullets = 5;
		
		private TypePickable weaponType = TypePickable.None;
		private int bulletDamage = 20;

		private ShootEventChannel shootEventChannel;

		public TypePickable WeaponType
		{
			set
			{
				weaponType = value;
				switch (weaponType)
				{
					case TypePickable.Shotgun:
						bulletDamage = 16;
						fireDelayInSeconds = 0.8f;
						break;
					case TypePickable.Uzi:
						bulletDamage = 12;
						fireDelayInSeconds = 0.2f;
						break;
					default:
						bulletDamage = 20;
						fireDelayInSeconds = 0.5f;
						break;
				}
			}
		}

		private float lastTimeShotInSeconds;

		public float FireDelayInSeconds
		{
			set { fireDelayInSeconds = value; }
		}

		public int NbOfShotgunBullets
		{
			set { nbOfShotgunBullets = value; }
		}

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

			shootEventChannel = GameObject.FindWithTag("GameController").GetComponent<ShootEventChannel>();
		}

		public void Shoot()
		{
			if (CanShoot)
			{
				if (weaponType == TypePickable.Shotgun)
				{
					ShootInCone();
				}
				else
				{
					ShootInLine();
				}

				NotifyShot();

				lastTimeShotInSeconds = Time.time;
			}
		}

		public float GetBulletSpeed()
		{
			return bulletPrefab.GetComponentInChildren<AnchoredMover>().MaxSpeed;
		}

		private void ShootInLine()
		{
			GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
			bullet.GetComponentInChildren<BulletController>().ConfigureLineShoot(bullet,bulletDamage);
		}

		private void ShootInCone()
		{
			for (int i = 0; i < nbOfShotgunBullets; ++i)
			{
				GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
				bullet.GetComponentInChildren<BulletController>().ConfigureConeShoot(bullet,bulletDamage);
			}
		}

		private void NotifyShot()
		{
			shootEventChannel.PublishWeaponShot(weaponType);
		}
	}
}