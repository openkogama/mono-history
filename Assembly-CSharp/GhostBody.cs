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
		currentAngularRotation = RotationWithInertia(angularMaxRotation);
		transform.Rotate(Vector3.up, currentAngularRotation * Time.deltaTime * 57.29578f, Space.Self);
	}

	private float RotationWithInertia(float desiredAngularRotation)
	{
		return Mathf.Lerp(currentAngularRotation, desiredAngularRotation, Time.deltaTime / timeBeforeTargetRotation);
	}
}
