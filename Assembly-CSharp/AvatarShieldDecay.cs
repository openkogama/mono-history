using UnityEngine;

public class AvatarShieldDecay : MonoBehaviour
{
	private const float decayCooldown = 1f;

	private const float decayAmount = 15f;

	private const float updateValueCooldown = 0.5f;

	private float decayTime;

	private MVRuntimeDataVariableClampedFloat shield;

	private float accumulatedShieldDecay;

	private float updateValueTime;

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
