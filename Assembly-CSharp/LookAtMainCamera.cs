using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
	protected void LateUpdate()
	{
		transform.rotation = MVGameControllerBase.CameraController.MainCamera.transform.rotation;
	}
}
