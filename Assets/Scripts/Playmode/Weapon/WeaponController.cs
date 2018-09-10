using System;
using Playmode.Bullet;
using Playmode.Entity.Movement;
using Playmode.Event;
using Playmode.Pickable.TypePickable;
using UnityEngine;

namespace Playmode.Weapon
{
	public class WeaponController : MonoBehaviour
	{
		[Header("Behavior")] [SerializeField] private GameObject bulletPrefab;
		
		[SerializeField] private float fireDelayInSeconds = 0.5f;
		[SerializeField] private int nbOfShotgunBullets = 5;
		
		private ShootEventChannel shootEventChannel;
		
		private TypePickable weaponType = TypePickable.None;
		
		private int bulletDamage = 20;
		private float lastTimeShotInSeconds;
		
		private bool CanShoot => Time.time - lastTimeShotInSeconds > fireDelayInSeconds;

		public TypePickable WeaponType
		{
			set
			{
				weaponType = value;
				
				switch (weaponType)
				{
					case TypePickable.Shotgun:
						bulletDamage = 16; //BEN_CORRECTION : Valeur magique.
						fireDelayInSeconds = 0.8f; //BEN_CORRECTION : Valeur magique.
						break;
					case TypePickable.Uzi:
						bulletDamage = 12; //BEN_CORRECTION : Valeur magique.
						fireDelayInSeconds = 0.2f; //BEN_CORRECTION : Valeur magique.
						break;
					default:
						bulletDamage = 20; //BEN_CORRECTION : Valeur magique.
						fireDelayInSeconds = 0.5f; //BEN_CORRECTION : Valeur magique.
						break;
				}
			}
		}

		public float FireDelayInSeconds
		{
			set { fireDelayInSeconds = value; }
		}

		public int NbOfShotgunBullets
		{
			set { nbOfShotgunBullets = value; }
		}

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

			//BEN_CORRECTION : Valeur magique devrait être en constante. Voir classe "Tags".
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
			BulletController.ConfigureLineShoot(bullet,bulletDamage);
		}

		private void ShootInCone()
		{
			for (int i = 0; i < nbOfShotgunBullets; ++i)
			{
				GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
				BulletController.ConfigureConeShoot(bullet,bulletDamage);
			}
		}

		private void NotifyShot()
		{
			shootEventChannel.PublishWeaponShot(weaponType);
		}
	}
}