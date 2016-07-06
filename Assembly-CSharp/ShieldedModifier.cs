using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class ShieldedModifier : AvatarModifier
{
	[SerializeField]
	private MeshRenderer shieldRenderer;

	[SerializeField]
	private float targetRimVisibility;

	[SerializeField]
	private float targetAlpha;

	[SerializeField]
	private RotatingShieldLine lineRenderer;

	private Material shieldMat;

	private bool readyToPlayEffect = true;

	private float prevHealth;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Shielded;

	public override bool EvaluateShouldBeAdded(Dictionary<AvatarModifierPackageType, AvatarModifier> modifiers)
	{
		return true;
	}

	protected override void OnActivated(Avatar target)
	{
		owner = target;
		transform.SetParent(owner.mvAvatar.Body.Transform);
		transform.localPosition += new Vector3(0f, 1f, 0f);
		shieldMat = shieldRenderer.material;
		prevHealth = owner.mvAvatar.Health.Value;
		MVRuntimeDataVariableClampedFloat health = owner.mvAvatar.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHealthChange));
		lineRenderer.Initialize();
		transform.SetParent(owner.mvAvatar.Body.GetSlotTransform(AvatarAccessorySlot.Torso));
		MVRuntimeDataVariable avatarRuntimeDataState = owner.mvAvatar.AvatarRuntimeDataState;
		avatarRuntimeDataState.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarRuntimeDataState.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChangedHandler));
		if (MVGameControllerBase.IPlayModeUI.InLobbyState)
		{
			gameObject.SetActive(value: false);
			lineRenderer.OnSetHidden();
		}
	}

	protected override void OnDeactivated(Avatar target)
	{
		MVRuntimeDataVariableClampedFloat health = owner.mvAvatar.Health;
		health.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(health.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnHealthChange));
		MVRuntimeDataVariable avatarRuntimeDataState = owner.mvAvatar.AvatarRuntimeDataState;
		avatarRuntimeDataState.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(avatarRuntimeDataState.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChangedHandler));
		UnityEngine.Object.Destroy(gameObject);
	}

	private void AvatarStateChangedHandler(object state)
	{
		switch ((AvatarRuntimeState)(byte)state)
		{
		case AvatarRuntimeState.Hidden:
			gameObject.SetActive(value: false);
			lineRenderer.OnSetHidden();
			break;
		case AvatarRuntimeState.Playing:
			gameObject.SetActive(value: true);
			lineRenderer.OnSetVisible();
			break;
		}
	}

	private void OnHealthChange(object floatHealth)
	{
		float num = (float)floatHealth;
		if (num < prevHealth && readyToPlayEffect)
		{
			readyToPlayEffect = false;
			StartCoroutine(MakeVisible(0.1f, 0.6f));
		}
		prevHealth = num;
	}

	private IEnumerator MakeVisible(float fadeInTime, float fadeOutTime)
	{
		float currRim = shieldMat.GetFloat("_Rim");
		Color currColor = shieldMat.GetColor("m_Color");
		for (float i = 0f; i < fadeInTime; i += Time.deltaTime / fadeInTime)
		{
			shieldMat.SetFloat("_Rim", Mathf.Lerp(currRim, targetRimVisibility, i));
			currColor.a = Mathf.Lerp(0f, targetAlpha, i);
			shieldMat.SetColor("m_Color", currColor);
			yield return null;
		}
		for (float i2 = 0f; i2 < fadeOutTime; i2 += Time.deltaTime / fadeOutTime)
		{
			shieldMat.SetFloat("_Rim", Mathf.Lerp(targetRimVisibility, currRim, i2));
			currColor.a = Mathf.Lerp(targetAlpha, 0f, i2);
			shieldMat.SetColor("m_Color", currColor);
			yield return null;
		}
		currColor.a = 0f;
		shieldMat.SetColor("m_Color", currColor);
		shieldMat.SetFloat("_Rim", 5f);
		readyToPlayEffect = true;
	}
}
