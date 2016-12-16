using UnityEngine;
using UnityEngine.UI;

public class GodzillaGUI : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve damageFlash;

	[SerializeField]
	private Image damageImage;

	private Color damageColor;

	private float timer = float.PositiveInfinity;

	private float magnitude;

	private void Awake()
	{
		damageColor = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, 0f);
		damageImage.color = damageColor;
	}

	private void Update()
	{
		if (FlashInProgress())
		{
			damageImage.color = new Color(damageColor.r, damageColor.g, damageColor.b, damageFlash.Evaluate(timer) * magnitude);
			timer += Time.deltaTime;
		}
		else
		{
			damageImage.color = damageColor;
		}
	}

	private bool FlashInProgress()
	{
		return timer < damageFlash[damageFlash.length - 1].time;
	}

	public void Flash(float strength = 1f)
	{
		magnitude = strength;
		timer = 0f;
	}

	public void AdaptiveFlash(float currentHealth, float previousHealth)
	{
		if (FlashInProgress())
		{
			timer /= 2f;
			magnitude += (previousHealth - currentHealth) / 100f;
		}
		else
		{
			timer = 0f;
			magnitude = (previousHealth - currentHealth) / 100f;
		}
		magnitude = Mathf.Clamp(magnitude, 0.1f, 1f);
	}
}
