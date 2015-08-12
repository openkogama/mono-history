using System;
using System.Collections.Generic;
using MV.Common;
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
		MVRuntimeDataVariable avatarRuntimeDataState = mvAvatar.AvatarRuntimeDataState;
		avatarRuntimeDataState.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarRuntimeDataState.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChangedHandler));
	}

	public void Detach()
	{
		visible = false;
		MVRuntimeDataVariableClampedFloat health = mvAvatar.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(HealthChangeHandler));
		MVRuntimeDataVariable invulnerable = mvAvatar.Invulnerable;
		invulnerable.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(invulnerable.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(InvulnerableChangedHandler));
		MVRuntimeDataVariable avatarRuntimeDataState = mvAvatar.AvatarRuntimeDataState;
		avatarRuntimeDataState.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(avatarRuntimeDataState.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChangedHandler));
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

	private void InvulnerableChangedHandler(object v)
	{
		bool flag = (bool)v;
		AvatarRuntimeState avatarRuntimeState = (AvatarRuntimeState)(byte)mvAvatar.AvatarRuntimeDataState.Value;
		if (flag && avatarRuntimeState == AvatarRuntimeState.Playing)
		{
			StartBlinking(BlinkType.Invulnerable, float.PositiveInfinity);
		}
		else
		{
			StopBlinking(BlinkType.Invulnerable);
		}
	}

	private void AvatarStateChangedHandler(object a)
	{
		if ((byte)a == 0)
		{
			StopBlinking(BlinkType.Invulnerable);
		}
		else if ((bool)mvAvatar.Invulnerable.Value)
		{
			StartBlinking(BlinkType.Invulnerable, float.PositiveInfinity);
		}
	}
}
