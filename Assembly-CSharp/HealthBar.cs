using UnityEngine;

public class HealthBar : MonoBehaviour
{
	public Transform healthPivot;

	public Transform oxygenPivot;

	public float Health
	{
		get
		{
			return healthPivot.localScale.x * 100f;
		}
		set
		{
			Vector3 localScale = healthPivot.localScale;
			localScale.x = Mathf.Clamp01(value / 100f);
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
}
