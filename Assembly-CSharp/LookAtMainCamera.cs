using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
	protected void LateUpdate()
	{
		transform.rotation = MVGameControllerBase.MainCameraManager.MainCamera.transform.rotation;
	}
}
