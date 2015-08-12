using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
	private Camera mainCamera;

	private void Start()
	{
		mainCamera = MVGameController.Game.CameraController.GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		transform.rotation = mainCamera.transform.rotation;
	}
}
