using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class HealingIndicator : MonoBehaviour
{
	[SerializeField]
	private Image healthOverlay;

	[SerializeField]
	private AnimationCurve curve;

	private float timer;

	private float animationDuration;

	private float targetTime;

	private void Awake()
	{
		transform.SetParent(null, worldPositionStays: false);
	}

	public void ShowHealing(float damageAmount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (damageAmount < 0f)
		{
			enabled = true;
			targetTime = curve.keys[curve.length - 1].time;
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		float a = curve.Evaluate(timer);
		Color color = healthOverlay.color;
		color.a = a;
		healthOverlay.color = color;
		if (timer >= targetTime)
		{
			timer = 0f;
			enabled = false;
		}
	}
}
