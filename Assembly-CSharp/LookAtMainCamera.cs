using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
	private Camera mainCamera;

	private void Start()
	{
		mainCamera = ((Component)MVGameController.Instance.Game.CameraController).GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.rotation = ((Component)mainCamera).transform.rotation;
	}
}
