using UnityEngine;

public class HealthBar : MonoBehaviour
{
	public Transform healthPivot;

	public Transform oxygenPivot;

	public float Health
	{
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = healthPivot.localScale;
			localScale.x = Mathf.Clamp01(value / 100f);
			healthPivot.localScale = localScale;
			Renderer componentInChildren = ((Component)healthPivot).GetComponentInChildren<Renderer>();
			if (Object.op_Implicit((Object)(object)componentInChildren))
			{
				componentInChildren.enabled = value > 0f;
			}
		}
	}

	public float Oxygen
	{
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = oxygenPivot.localScale;
			localScale.x = Mathf.Clamp01(value / 100f);
			oxygenPivot.localScale = localScale;
			Renderer componentInChildren = ((Component)oxygenPivot).GetComponentInChildren<Renderer>();
			if (Object.op_Implicit((Object)(object)componentInChildren))
			{
				componentInChildren.enabled = value > 0f;
			}
		}
	}
}
