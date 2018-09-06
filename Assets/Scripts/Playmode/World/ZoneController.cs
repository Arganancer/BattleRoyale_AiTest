using UnityEngine;

namespace Playmode.World
{
	public class ZoneController : MonoBehaviour
	{
		private const float SpriteScaleSize = 0.0625f;
		private const int MaxShrinkingSize = 2;
		private const int MaxShrinkSpeedBuffer = 100;
		
		[SerializeField] private float timeBufferToMoveZone = 30;
		[SerializeField] private float startingRadiusZoneSize = 10;
		[SerializeField] private int minimunSizeShrink = 2;
		[SerializeField] private int maximumSizeShrink = 3;
		[SerializeField] private float sizeReduction = 0.5f;
		
		private CircleCollider2D zoneCollider2D;
		private GameObject zoneRenderer;
		
		private float nextRadius;
		private float timeOfLastShrink;

		public Vector2 DistanceOffSet { get; private set; }

		public float CurrentRadius { get; private set; }

		public bool ZoneIsNotShrinking { get; private set; } = true;

		private void Awake()
		{
			zoneCollider2D = transform.root.GetComponentInChildren<CircleCollider2D>();
			zoneCollider2D.radius = startingRadiusZoneSize;
			nextRadius = startingRadiusZoneSize;
			CurrentRadius = nextRadius;
	
			zoneRenderer = transform.root.GetComponentInChildren<SpriteRenderer>().transform.gameObject;
			zoneRenderer.transform.localScale = new Vector3(1.26f, 1.26f, 0);
			
			DistanceOffSet = new Vector2();
		}
	
		private void Update()
		{
			ChangeZonePositionAndSize();
			zoneRenderer.transform.Rotate(Vector3.back);
		}
	
		private void ChangeZonePositionAndSize()
		{
			if (Time.time - timeOfLastShrink > timeBufferToMoveZone && GetCurrentZoneRadius() <= nextRadius)
			{
				if (GetCurrentZoneRadius() > MaxShrinkingSize) 
					ChangeZoneColliderOffset();
	
				timeOfLastShrink = Time.time;
			}
			else if (GetCurrentZoneRadius() > nextRadius && zoneRenderer.transform.localScale.x > SpriteScaleSize
					 && GetCurrentZoneRadius() - sizeReduction > 1) //the 1 is there to make sure the radius doesn't go negative after the reduction.
			{
				ShrinkZone();
				ZoneIsNotShrinking = false;
			}
			else
			{
				CurrentRadius = nextRadius;
				ZoneIsNotShrinking = true;
			}
		}
	
		private void ShrinkZone()
		{
			zoneCollider2D.radius -= sizeReduction/MaxShrinkSpeedBuffer;
			
			ChangeSpriteScale();
			ChangeSpritePosition();
		}
	
		private void ChangeZoneColliderOffset()
		{
			var offset = GetRandomZoneOffSetWithinCurrentCircle();
			zoneCollider2D.offset = offset;
		}
	
		private float GetCurrentZoneRadius()
		{
			return zoneCollider2D.radius;
		}
	
		private Vector2 GetRandomZoneOffSetWithinCurrentCircle()
		{
			nextRadius = GetCurrentZoneRadius() - GetRandomZoneRadiusSize();
			var maxOffset = GetCurrentZoneRadius() - nextRadius;
			
			var y = (int)Random.Range(1, maxOffset);
			var x = (int)Random.Range(1, maxOffset);
			
			return new Vector2(x, y);
		}
	
		private int GetRandomZoneRadiusSize()
		{
			return Random.Range(minimunSizeShrink, maximumSizeShrink);
		}
	
		private float GetCurrentZoneSpriteScale()
		{
			return zoneRenderer.transform.localScale.x;
		}
	
		private void ChangeSpriteScale()
		{
			zoneRenderer.transform.localScale = 
				new Vector3(
					GetCurrentZoneSpriteScale() - SpriteScaleSize / MaxShrinkSpeedBuffer,
					GetCurrentZoneSpriteScale() - SpriteScaleSize / MaxShrinkSpeedBuffer,
					0);
		}
	
		private void ChangeSpritePosition()
		{
			DistanceOffSet = zoneCollider2D.offset;
			zoneRenderer.transform.localPosition =
				zoneCollider2D.offset;
		}
	}
}