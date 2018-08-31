using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.UIElements;

/// <summary>
/// TODO: Add "private" to functions
/// </summary>
public class CameraController : MonoBehaviour
{
	private Camera mainCam;
	private CircleCollider2D zoneObject;
	private float middleHeight, middleWidth;
	private Vector2 movement;
	private int cameraMovementSpeed = 5;
	private bool isMoving = false;
	private void Awake()
	{
		mainCam = Camera.main;
		zoneObject = GameObject.Find("Zone").GetComponentInChildren<CircleCollider2D>();
		float height = mainCam.orthographicSize*2f;
		float width = height * mainCam.aspect;
		middleHeight = height/2;
		middleWidth = width/2;
		movement = Vector2.zero;
	}

	public void OnResetButtonClick()
	{
		mainCam.transform.position = 
			new Vector3(zoneObject.offset.x*10,
			zoneObject.offset.y*10,
			-10);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		MoveCamera();
		
	}

	void MoveCamera()
	{
		CheckKeyDown();
		CheckKeyUp();
		mainCam.transform.position = new Vector3(mainCam.transform.position.x+ movement.x,
			mainCam.transform.position.y+movement.y,-10);
	}

	void CheckKeyDown()
	{
		if (CheckIfCameraCanGoUp() && (Input.GetKeyDown(KeyCode.W) || isMoving))
		{
			isMoving = true;
			movement.y = cameraMovementSpeed*Time.deltaTime;
		}

		if (CheckIfCameraCanGoDown() && (Input.GetKeyDown(KeyCode.S) || isMoving))
		{
			isMoving = true;
			movement.y = -cameraMovementSpeed*Time.deltaTime;
		}
		if (CheckIfCameraCanGoRight() && (Input.GetKeyDown(KeyCode.D) || isMoving))
		{
			isMoving = true;
			movement.x = cameraMovementSpeed*Time.deltaTime;
		}

		if (CheckIfCameraCanGoLeft() && (Input.GetKeyDown(KeyCode.A) || isMoving))
		{
			isMoving = true;
			movement.x = -cameraMovementSpeed*Time.deltaTime;
		}
	}

	void CheckKeyUp()
	{
		if (Input.GetKeyUp(KeyCode.W))
		{
			isMoving = false;
			movement.y =0;
		}

		if (Input.GetKeyUp(KeyCode.S))
		{
			isMoving = false;
			movement.y =0;
		}
		if (Input.GetKeyUp(KeyCode.D))
		{
			isMoving = false;
			movement.x =0;
		}

		if (Input.GetKeyUp(KeyCode.A))
		{
			isMoving = false;
			movement.x =0;
		}
	}

	bool CheckIfCameraCanGoUp()
	{
		return mainCam.transform.position.y + middleHeight + cameraMovementSpeed < zoneObject.radius;
	}
	
	bool CheckIfCameraCanGoDown()
	{
		return mainCam.transform.position.y - middleHeight - cameraMovementSpeed > -zoneObject.radius;
	}

	bool CheckIfCameraCanGoRight()
	{
		return mainCam.transform.position.x + middleHeight + cameraMovementSpeed < zoneObject.radius;
	}

	bool CheckIfCameraCanGoLeft()
	{
		return mainCam.transform.position.y - middleHeight - cameraMovementSpeed > -zoneObject.radius;
	}
}
