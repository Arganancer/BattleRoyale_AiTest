using System;
using Playmode.Entity.Movement;
using Playmode.Event;
using Playmode.Pickable.TypePickable;
using Playmode.Sound;
using UnityEngine;

namespace Playmode.Weapon
{
	public class WeaponController : MonoBehaviour
	{
		[Header("Behavior")] [SerializeField] private GameObject bulletPrefab;
		[SerializeField] private float fireDelayInSeconds = 0.3f;
		[SerializeField] private float angleBetweenBullet = 50f;
		[SerializeField] private int nbOfShotgunBullets = 5;
		private TypePickable weaponType = TypePickable.None;

		private ShootEventChannel shootEventChannel;
		private PlaySoundOnShoot playSoundOnShoot;

		public TypePickable WeaponType
		{
			get { return weaponType; }
			set { weaponType = value; }
		}
		private float lastTimeShotInSeconds;

		public float FireDelayInSeconds
		{
			get { return fireDelayInSeconds; }
			set { fireDelayInSeconds = value; }
		}

		public int NbOfShotgunBullets
		{
			get { return nbOfShotgunBullets; }
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
			return bulletPrefab.GetComponentInChildren<AnchoredMover>().GetCurrentSpeed();
		}

		public void ShootInLine()
		{
			Instantiate(bulletPrefab, transform.position, transform.rotation);
		}

		public void ShootInCone()
		{
			for (int i = 0; i < nbOfShotgunBullets; ++i)
			{
				GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
				if (i % 2 == 0)
				{
					bullet.transform.Rotate(Vector3.forward*angleBetweenBullet*i,Space.Self);
				}
				else
				{
					bullet.transform.Rotate(Vector3.back*angleBetweenBullet*i,Space.Self);
				}
			}
		}

		private void NotifyShot()
		{
			shootEventChannel.Publish();
		}
	}
}