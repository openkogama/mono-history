using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
	private void LateUpdate()
	{
		transform.rotation = MVGameControllerBase.CameraController.MainCamera.transform.rotation;
	}
}
