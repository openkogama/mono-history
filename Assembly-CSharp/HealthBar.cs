using UnityEngine;

public class HealthBar : MonoBehaviour
{
	public Transform leftPivot;

	public float Health
	{
		set
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			leftPivot.localScale = new Vector3(Mathf.Clamp01(value / 100f), 1f, 1f);
		}
	}
}
