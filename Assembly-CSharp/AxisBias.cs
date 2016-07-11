using UnityEngine;

public class AxisBias : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve horizontalBias;

	public Vector3 GetBiasedVector(Vector3 inputVector)
	{
		float magnitude = inputVector.magnitude;
		Vector3 normalized = inputVector.normalized;
		float num = Vector3.Dot(Vector3.up, normalized);
		if (num > 0f)
		{
			return GetBiased(num, normalized, Vector3.up) * magnitude;
		}
		float num2 = Vector3.Dot(Vector3.down, normalized);
		if (num2 > 0f)
		{
			return GetBiased(num2, normalized, Vector3.down) * magnitude;
		}
		return inputVector;
	}

	private Vector3 GetBiased(float dotVal, Vector3 normalizedInputVector, Vector3 biasVector)
	{
		float t = horizontalBias.Evaluate(dotVal);
		Vector3 result = Vector3.Lerp(normalizedInputVector, biasVector, t);
		result.Normalize();
		return result;
	}
}
