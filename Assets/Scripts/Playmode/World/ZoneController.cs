using UnityEngine;

public class ZoneController : MonoBehaviour
{
	private const float SPRITE_SCALE_SIZE = 0.0625f;
	private const int MAX_SHRINKING_SIZE = 2;
	private const int MAX_SHRINK_SPEED_BUFFER = 100;
	private const string ZONE_RENDERER_OBJECT = "ZoneRenderer";
	
	[SerializeField] float timeBufferToMoveZone = 120f;
	[SerializeField] private int startingRadiusZoneSize = 10;
	[SerializeField] private int minimunSizeShrink = 2;
	[SerializeField] private int maximumSizeShrink = 3;
	[SerializeField] private float sizeReduction = 0.5f;
	
	private CircleCollider2D zoneCollider2D;
	private GameObject zoneRenderer;
	
	private float nextRadius;
	private float timeOfLastShrink;
	private float currentRadius;
	private bool zoneIsNotShrinking = true;

	public float CurrentRadius => currentRadius;
	public bool ZoneIsNotShrinking => zoneIsNotShrinking;
	
	private void Awake()
	{
		zoneCollider2D = transform.root.GetComponentInChildren<CircleCollider2D>();
		zoneCollider2D.radius = startingRadiusZoneSize;
		nextRadius = startingRadiusZoneSize;
		currentRadius = nextRadius;

		zoneRenderer = transform.root.Find(ZONE_RENDERER_OBJECT).gameObject;
		zoneRenderer.transform.localScale = new Vector3(1.26f,1.26f,0);
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
			if (GetCurrentZoneRadius() > MAX_SHRINKING_SIZE)
			{
				ChangeZoneColliderOffset();
			}

			timeOfLastShrink = Time.time;
		}
		else if (GetCurrentZoneRadius() > nextRadius && zoneRenderer.transform.localScale.x > SPRITE_SCALE_SIZE
		         && GetCurrentZoneRadius() - sizeReduction >1) //the 1 is there to make sure the radius doesn't go negative after the reduction.
		{
			ShrinkZone();
			zoneIsNotShrinking = false;
		}
		else
		{
			currentRadius = nextRadius;
			zoneIsNotShrinking = true;
		}
	}

	void ShrinkZone()
	{
		zoneCollider2D.radius -= sizeReduction/MAX_SHRINK_SPEED_BUFFER;
		
		ChangeSpriteScale();
		ChangeSpritePosition();
	}
	void ChangeZoneColliderOffset()
	{
		Vector2 offset = GetRandomZoneOffSetWithinCurrentCircle();
		zoneCollider2D.offset = offset;
	}


	float GetCurrentZoneRadius()
	{
		return zoneCollider2D.radius;
	}

	Vector2 GetRandomZoneOffSetWithinCurrentCircle()
	{
		nextRadius = GetCurrentZoneRadius() - GetRandomZoneRadiusSize();
		float maxOffset = GetCurrentZoneRadius() - nextRadius;
		
		int y = (int)Random.Range(1, maxOffset);
		int x = (int)Random.Range(1, maxOffset);
		
		return new Vector2(x,y);

	}

	int GetRandomZoneRadiusSize()
	{
		return Random.Range(minimunSizeShrink, maximumSizeShrink);
	}

	float GetCurrentZoneSpriteScale()
	{
		return zoneRenderer.transform.localScale.x;
	}

	void ChangeSpriteScale()
	{
		zoneRenderer.transform.localScale = 
			new Vector3(
				GetCurrentZoneSpriteScale()- SPRITE_SCALE_SIZE/MAX_SHRINK_SPEED_BUFFER,
				GetCurrentZoneSpriteScale()- SPRITE_SCALE_SIZE/MAX_SHRINK_SPEED_BUFFER,
				0);
	}

	void ChangeSpritePosition()
	{
		zoneRenderer.transform.localPosition =
			zoneCollider2D.offset;
	}
}
