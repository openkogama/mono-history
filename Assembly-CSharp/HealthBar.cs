using UnityEngine;

public class HealthBar : MonoBehaviour
{
	public Transform healthPivot;

	public Transform oxygenPivot;

	private float healthBoost = 1f;

	private float maxHealth = 100f;

	public float MaxHealth
	{
		set
		{
			maxHealth = value;
			SetScaleFromHealth(Health);
		}
	}

	public float Health
	{
		get
		{
			return healthPivot.localScale.x * maxHealth;
		}
		set
		{
			SetScaleFromHealth(value);
		}
	}

	public float Oxygen
	{
		set
		{
			Vector3 localScale = oxygenPivot.localScale;
			localScale.x = Mathf.Clamp01(value / 100f);
			oxygenPivot.localScale = localScale;
		}
	}

	private void SetScaleFromHealth(float value)
	{
		Vector3 localScale = healthPivot.localScale;
		localScale.x = Mathf.Clamp01(value / maxHealth);
		healthPivot.localScale = localScale;
	}

	private float GetBoostedHealth(float defaultHealth)
	{
		return defaultHealth * healthBoost;
	}
}
