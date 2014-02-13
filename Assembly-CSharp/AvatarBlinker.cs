using System;
using System.Collections.Generic;
using UnityEngine;

public class AvatarBlinker : BlinkerBase
{
	public Color blinkInvulnerableColor = new Color(29f, 108f, 219f);

	public Color blinkDamageColor = new Color(251f, 0f, 0f);

	public Color blinkHealthBoostColor = new Color(233f, 249f, 9f);

	public Color blinkPoisonColor = new Color(0f, 255f, 20f);

	public Color blinkFrozenColor = new Color(200f, 230f, 255f);

	private MVAvatar mvAvatar;

	private float previousBlinkHealth = 100f;

	public AvatarBlinker()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
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
				BlinkType.Invulnerable,
				new Blinker(3f, blinkMaterial, blinkInvulnerableColor)
			},
			{
				BlinkType.Poison,
				new Blinker(2f, blinkMaterial, blinkPoisonColor)
			},
			{
				BlinkType.Frozen,
				new Blinker(3f, blinkMaterial, blinkFrozenColor)
			}
		};
	}

	public void Attach(MVAvatar mvAvatar)
	{
		this.mvAvatar = mvAvatar;
		MVRuntimeDataVariableClampedFloat health = mvAvatar.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(HealthChangeHandler));
		MVRuntimeDataVariable invulnerable = mvAvatar.Invulnerable;
		invulnerable.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(invulnerable.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(InvulnerableChangedHandler));
	}

	public void Detach()
	{
		visible = false;
		MVRuntimeDataVariableClampedFloat health = mvAvatar.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(HealthChangeHandler));
		MVRuntimeDataVariable invulnerable = mvAvatar.Invulnerable;
		invulnerable.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(invulnerable.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(InvulnerableChangedHandler));
	}

	public void HealthChangeHandler(object v)
	{
		float num = (float)v;
		if (num < previousBlinkHealth)
		{
			StartBlinking(BlinkType.Damage, 0.5f);
		}
		previousBlinkHealth = num;
	}

	public void InvulnerableChangedHandler(object v)
	{
		if ((bool)v)
		{
			StartBlinking(BlinkType.Invulnerable, float.PositiveInfinity);
		}
		else
		{
			StopBlinking(BlinkType.Invulnerable);
		}
	}
}
