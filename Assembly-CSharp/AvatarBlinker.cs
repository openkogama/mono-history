using System;
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

	private MVAvatar mvAvatar;

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

	public void Attach(MVAvatar mvAvatar)
	{
		this.mvAvatar = mvAvatar;
		MVRuntimeDataVariableClampedFloat health = mvAvatar.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(HealthChangeHandler));
		MVRuntimeDataVariableClampedFloat shield = mvAvatar.Shield;
		shield.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(shield.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(ShieldChangeHandler));
	}

	public void Detach()
	{
		visible = false;
		MVRuntimeDataVariableClampedFloat health = mvAvatar.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(HealthChangeHandler));
		MVRuntimeDataVariableClampedFloat shield = mvAvatar.Shield;
		shield.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(shield.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(ShieldChangeHandler));
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
		if (mvAvatar.CurrentPickup.IsInFirstPersonMode && mvAvatar.Avatar.IsLocal)
		{
			StopBlinking(blinkType);
		}
		else if (currentValue < previousValue)
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
