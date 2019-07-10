using System.Collections.Generic;
using UnityEngine;

public class AvatarBlinker : BlinkerBase
{
	public Color blinkDamageColor = new Color(251f, 0f, 0f);

	public Color blinkHealthBoostColor = new Color(233f, 249f, 9f);

	public Color blinkPoisonColor = new Color(0f, 255f, 20f);

	public Color blinkFrozenColor = new Color(200f, 230f, 255f);

	public Color blinkHealingColor = new Color(233f, 249f, 9f);

	public Color blinkShieldColor = new Color(25f, 25f, 112f);

	private float previousBlinkHealth = 100f;

	private float previousBlinkShield;

	private void Awake()
	{
		blinkers = new Dictionary<BlinkType, Blinker>
		{
			{
				BlinkType.Damage,
				new Blinker(4f, blinkMaterial, blinkDamageColor)
			},
			{
				BlinkType.Health,
				new Blinker(5f, blinkMaterial, blinkHealthBoostColor)
			},
			{
				BlinkType.Poison,
				new Blinker(2f, blinkMaterial, blinkPoisonColor)
			},
			{
				BlinkType.Frozen,
				new Blinker(3f, blinkMaterial, blinkFrozenColor)
			},
			{
				BlinkType.Healing,
				new Blinker(2f, blinkMaterial, blinkHealingColor)
			},
			{
				BlinkType.ShieldDamage,
				new Blinker(2f, blinkMaterial, blinkShieldColor)
			}
		};
	}

	public void EnableBlinking()
	{
		visible = true;
	}

	public void DisableBlinking()
	{
		visible = false;
	}

	public void HealthChangeHandler(object v)
	{
		float currentValue = (float)v;
		HandleDamageBlinking(previousBlinkHealth, currentValue, BlinkType.Damage);
		previousBlinkHealth = currentValue;
	}

	public void UpdateBlinking()
	{
		DoBlinking();
	}

	public override void LateUpdate()
	{
	}

	public void ShieldChangeHandler(object v)
	{
		float currentValue = (float)v;
		HandleDamageBlinking(previousBlinkShield, currentValue, BlinkType.ShieldDamage);
		previousBlinkShield = currentValue;
	}

	private void HandleDamageBlinking(float previousValue, float currentValue, BlinkType blinkType)
	{
		if (currentValue < previousValue)
		{
			StartBlinking(blinkType, 0.5f);
		}
	}

	public void SetPreviousHealth(float health)
	{
		previousBlinkHealth = health;
	}

	public void SetPreviousShield(float shield)
	{
		previousBlinkShield = shield;
	}
}
