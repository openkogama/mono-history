using UnityEngine;

public class SunPlane : MonoBehaviour
{
	public Light mainLight;

	public Transform sunPlane;

	private void OnPreRender()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		sunPlane.position = ((Component)Camera.main).transform.position - ((Component)mainLight).transform.forward * 30f;
		sunPlane.LookAt(((Component)Camera.main).transform.position);
		sunPlane.RotateAround(sunPlane.right, 90f);
	}
}
