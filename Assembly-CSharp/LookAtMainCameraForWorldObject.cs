using UnityEngine;

public class LookAtMainCameraForWorldObject : MonoBehaviour
{
	private void LateUpdate()
	{
		transform.up = -Camera.main.transform.forward;
	}
}
