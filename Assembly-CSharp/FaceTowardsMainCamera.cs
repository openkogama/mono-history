using UnityEngine;

public class FaceTowardsMainCamera : MonoBehaviour
{
	private void LateUpdate()
	{
		transform.LookAt(MVGameControllerBase.MainCameraManager.MainCamera.transform);
		transform.Rotate(new Vector3(90f, 0f, 0f));
	}
}
