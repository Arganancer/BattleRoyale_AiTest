using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.UIElements;

public class CameraController : MonoBehaviour
{
	private const int MAX_SHRINKING_SIZE = 2;
	private const string MOUSE_WHEEL = "Mouse ScrollWheel";
	
	private Camera mainCam;
	private CircleCollider2D zoneObject;
	
	private int cameraMovementSpeed = 5;
	private int cameraSizeRatio = 20;
	private float cameraHeight, cameraWidth;
	private float middleHeight, middleWidth;
	private int sizeOfCamera = 20;
	private Vector2 movement;
	
	private void Awake()
	{
		mainCam = Camera.main;
		
		mainCam.orthographicSize = sizeOfCamera;
		cameraHeight = mainCam.orthographicSize*2f;
		cameraWidth = cameraHeight * mainCam.aspect;
		middleHeight = cameraHeight/2/cameraSizeRatio;
		middleWidth = cameraWidth/2/cameraSizeRatio;
		movement = Vector2.zero;
		
		zoneObject = GameObject.Find("Zone").GetComponentInChildren<CircleCollider2D>();
	}

	/// <summary>
	/// Used by unity UI
	/// </summary>
	public void OnResetButtonClick()
	{
		mainCam.transform.position = 
			new Vector3(zoneObject.offset.x*10,
			zoneObject.offset.y*10,
			-10);
	}
	
	// Update is called once per frame
	private void Update () {
		
		cameraHeight = mainCam.orthographicSize*2f;
		cameraWidth = cameraHeight * mainCam.aspect;
		middleHeight = cameraHeight/2/cameraSizeRatio;
		middleWidth = cameraWidth/2/cameraSizeRatio;
		MoveCamera();
		
	}

	private void MoveCamera()
	{
		CheckKey();
		mainCam.transform.Translate(movement);
		CheckAndMoveCameraIfNeeded();
	}

	private void CheckKey()
	{
		movement.x = 0;
		movement.y = 0;
		
		if (CheckIfCameraCanGoUp() && Input.GetKey(KeyCode.W))
		{
			movement.y = cameraMovementSpeed*Time.deltaTime;
		}

		if (CheckIfCameraCanGoDown() && Input.GetKey(KeyCode.S))
		{
			movement.y = -cameraMovementSpeed*Time.deltaTime;
		}
		
		if (CheckIfCameraCanGoRight() && Input.GetKey(KeyCode.D))
		{
			movement.x = cameraMovementSpeed*Time.deltaTime;
		}

		if (CheckIfCameraCanGoLeft() && Input.GetKey(KeyCode.A))
		{
			movement.x = -cameraMovementSpeed*Time.deltaTime;
		}

		if (Input.GetAxis(MOUSE_WHEEL) > 0f && mainCam.orthographicSize >0)
		{
			--mainCam.orthographicSize;
		}
		else if (Input.GetAxis(MOUSE_WHEEL) < 0f)
		{
			++mainCam.orthographicSize;
		}
	}

	private bool CheckIfCameraCanGoUp()
	{
		return mainCam.transform.position.y + middleHeight + cameraMovementSpeed < zoneObject.radius*cameraSizeRatio/2;
	}
	
	private bool CheckIfCameraCanGoDown()
	{
		return mainCam.transform.position.y - middleHeight - cameraMovementSpeed > -zoneObject.radius*cameraSizeRatio/2;
	}

	private bool CheckIfCameraCanGoRight()
	{
		return mainCam.transform.position.x + middleWidth + cameraMovementSpeed < zoneObject.radius*cameraSizeRatio/2-30;
	}

	private bool CheckIfCameraCanGoLeft()
	{
		return mainCam.transform.position.x - middleWidth - cameraMovementSpeed > -zoneObject.radius*cameraSizeRatio/2;
	}

	private void CheckAndMoveCameraIfNeeded()
	{
		if ( zoneObject.radius<= MAX_SHRINKING_SIZE)
		{
			mainCam.transform.position = new Vector3(zoneObject.offset.x*10,zoneObject.offset.y*10,-10);
		}
	}
}
