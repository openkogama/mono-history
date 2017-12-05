using UnityEngine;

public class ShieldBar : MonoBehaviour
{
	private const float maxShieldValue = 100f;

	public Transform shieldPivot;

	private float interpolateTowardsShield;

	private float previousShieldValue;

	private float elapsedInterpolationTime;

	public float Shield
	{
		get
		{
			return shieldPivot.localScale.x * 100f;
		}
		set
		{
			interpolateTowardsShield = value;
			elapsedInterpolationTime = 0f;
		}
	}

	private void Update()
	{
		elapsedInterpolationTime += Time.deltaTime;
		float num = (previousShieldValue = Mathf.Lerp(previousShieldValue, interpolateTowardsShield, elapsedInterpolationTime));
		Vector3 localScale = shieldPivot.localScale;
		localScale.x = Mathf.Clamp01(num / 100f);
		shieldPivot.localScale = localScale;
	}
}
