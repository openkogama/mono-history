using UnityEngine;

public class AvatarShieldDecay : MonoBehaviour
{
	private float decayTime;

	private const float decayCooldown = 1f;

	private MVRuntimeDataVariableClampedFloat shield;

	private const float decayAmount = 15f;

	private float accumulatedShieldDecay;

	private float updateValueTime;

	private const float updateValueCooldown = 0.5f;

	public void Init(MVRuntimeDataVariableClampedFloat shield)
	{
		this.shield = shield;
	}

	private void Update()
	{
		if (decayTime < Time.time)
		{
			accumulatedShieldDecay += 15f * Time.deltaTime;
		}
		if (updateValueTime < Time.time)
		{
			updateValueTime = Time.time + 0.5f;
			shield.Value -= accumulatedShieldDecay;
			accumulatedShieldDecay = 0f;
		}
	}

	public void ResetDecayTimer()
	{
		decayTime = Time.time + 1f;
		updateValueTime = Time.time + 0.5f + 1f;
	}
}
