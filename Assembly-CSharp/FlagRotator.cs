using UnityEngine;

public class FlagRotator : MonoBehaviour
{
	public MeshRenderer flag;

	public float rotationSpeed = 1f;

	private void Update()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)flag != (Object)null)
		{
			((Component)flag).transform.RotateAround(Vector3.up, Time.deltaTime * rotationSpeed);
		}
	}
}
