using UnityEngine;

public class HealthBar : MonoBehaviour
{
	public Transform healthPivot;

	public Transform oxygenPivot;

	public float MaxHealth { get; set; }

	public float Health
	{
		get
		{
			return healthPivot.localScale.x * MaxHealth;
		}
		set
		{
			Vector3 localScale = healthPivot.localScale;
			localScale.x = Mathf.Clamp01(value / MaxHealth);
			healthPivot.localScale = localScale;
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
		localScale.x = Mathf.Clamp01(value / MaxHealth);
		healthPivot.localScale = localScale;
	}
}
