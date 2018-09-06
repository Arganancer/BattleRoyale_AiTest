using UnityEngine;

namespace Playmode.Interface.Camera
{
	public class CameraController : MonoBehaviour
	{
		private const int MaxShrinkingSize = 2;
		private const string MouseWheel = "Mouse ScrollWheel";
	
		private UnityEngine.Camera mainCam;
		private CircleCollider2D zoneObject;

		private const int CameraMovementSpeed = 100;
		private const int CameraSizeRatio = 90;
		private float cameraHeight, cameraWidth;
		private float middleHeight, middleWidth;
		private const int SizeOfCamera = 90;
		private const float CameraHeightAdjuster = 5f;
		private Vector2 movement;
		private const bool CanZoom = true;

		/*public bool CanZoom
		{
			set { canZoom = value; }
		}*/
	
		private void Awake()
		{
			mainCam = UnityEngine.Camera.main;
		
			mainCam.orthographicSize = SizeOfCamera;
			cameraHeight = mainCam.orthographicSize*2f;
			cameraWidth = cameraHeight * mainCam.aspect;
			middleHeight = cameraHeight/2/CameraSizeRatio;
			middleWidth = cameraWidth/2/CameraSizeRatio;
			movement = Vector2.zero;
		
			zoneObject = GameObject.Find("Zone").GetComponentInChildren<CircleCollider2D>();
		}
	
		// Update is called once per frame
		private void Update () {
		
			cameraHeight = mainCam.orthographicSize*2f;
			cameraWidth = cameraHeight * mainCam.aspect;
			middleHeight = cameraHeight/2/CameraSizeRatio;
			middleWidth = cameraWidth/2/CameraSizeRatio;
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
				movement.y = CameraMovementSpeed*Time.deltaTime;
			}

			if (CheckIfCameraCanGoDown() && Input.GetKey(KeyCode.S))
			{
				movement.y = -CameraMovementSpeed*Time.deltaTime;
			}
		
			if (CheckIfCameraCanGoRight() && Input.GetKey(KeyCode.D))
			{
				movement.x = CameraMovementSpeed*Time.deltaTime;
			}

			if (CheckIfCameraCanGoLeft() && Input.GetKey(KeyCode.A))
			{
				movement.x = -CameraMovementSpeed*Time.deltaTime;
			}

			if (Input.GetAxis(MouseWheel) > 0f && mainCam.orthographicSize >0 && CanZoom)
			{
				if (mainCam.orthographicSize - CameraHeightAdjuster >0)
				mainCam.orthographicSize -= CameraHeightAdjuster;
			}
			else if (Input.GetAxis(MouseWheel) < 0f && CanZoom)
			{
				mainCam.orthographicSize += CameraHeightAdjuster;
			}
		}

		private bool CheckIfCameraCanGoUp()
		{
			return mainCam.transform.position.y + middleHeight + CameraMovementSpeed < zoneObject.radius*CameraSizeRatio/2;
		}
	
		private bool CheckIfCameraCanGoDown()
		{
			return mainCam.transform.position.y - middleHeight - CameraMovementSpeed > -zoneObject.radius*CameraSizeRatio/2;
		}

		private bool CheckIfCameraCanGoRight()
		{
			return mainCam.transform.position.x + middleWidth + CameraMovementSpeed < zoneObject.radius*CameraSizeRatio/2-30;
		}

		private bool CheckIfCameraCanGoLeft()
		{
			return mainCam.transform.position.x - middleWidth - CameraMovementSpeed > -zoneObject.radius*CameraSizeRatio/2;
		}

		private void CheckAndMoveCameraIfNeeded()
		{
			if ( zoneObject.radius<= MaxShrinkingSize)
			{
				ResetCameraPosition();
			}
		}

		private void ResetCameraPosition()
		{
			mainCam.transform.position = new Vector3(zoneObject.offset.x*10,zoneObject.offset.y*10,-10);
		}
	}
}
