using UnityEngine;

public class GhostBody : MonoBehaviour
{
	private float angularMaxRotationBase = 13f;

	private float angularMaxRotation = 5f;

	private float currentAngularRotation;

	private float timeBeforeTargetRotation = 0.2f;

	private void Update()
	{
		UpdateRotation();
	}

	public void SetRotationSpeed(float rotationSpeed)
	{
		angularMaxRotation = angularMaxRotationBase * rotationSpeed;
	}

	private void UpdateRotation()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		currentAngularRotation = RotationWithInertia(angularMaxRotation);
		((Component)this).transform.RotateAroundLocal(Vector3.up, currentAngularRotation * Time.deltaTime);
	}

	private float RotationWithInertia(float desiredAngularRotation)
	{
		return Mathf.Lerp(currentAngularRotation, desiredAngularRotation, Time.deltaTime / timeBeforeTargetRotation);
	}
}
