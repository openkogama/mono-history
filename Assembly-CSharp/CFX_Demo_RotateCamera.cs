using UnityEngine;

public class CFX_Demo_RotateCamera : MonoBehaviour
{
	public static bool rotating = true;

	public float speed = 30f;

	public Transform rotationCenter;

	private void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (rotating)
		{
			((Component)this).transform.RotateAround(rotationCenter.position, Vector3.up, speed * Time.deltaTime);
		}
	}
}
