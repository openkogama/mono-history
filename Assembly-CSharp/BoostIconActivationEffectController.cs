using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoostIconActivationEffectController : MonoBehaviour
{
	private enum EffectState
	{
		Inactive,
		Showing,
		Fading
	}

	[Serializable]
	private struct BoosterIcons
	{
		public BoostType type;

		public GameObject icon;
	}

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private List<BoosterIcons> boosterIcons;

	[SerializeField]
	private float showingDuration;

	[SerializeField]
	private float fadingDuration;

	[SerializeField]
	private float startNewEffectDelay;

	[SerializeField]
	private float fadingSlideAmount;

	private EffectState currentState;

	private float stateStartTime;

	private float originalYPosition;

	private bool haveStartedNewEffect;

	private BoostType boostType;

	private UnityAction startNewEffect;

	public void Initialize(BoostType type, UnityAction startNewEffect)
	{
		for (int i = 0; i < boosterIcons.Count; i++)
		{
			bool active = boosterIcons[i].type == type;
			boosterIcons[i].icon.SetActive(active);
		}
		originalYPosition = transform.localPosition.y;
		boostType = type;
		this.startNewEffect = startNewEffect;
	}

	public void Activate()
	{
		if (MVGameControllerBase.LocalPlayer.BoostController.IsBoostActive(boostType))
		{
			canvasGroup.alpha = 1f;
			SetState(EffectState.Showing);
			haveStartedNewEffect = false;
			Transform transform = base.transform;
			Vector3 localPosition = new Vector3(base.transform.localPosition.x, originalYPosition, base.transform.localPosition.z);
			base.transform.localPosition = localPosition;
			transform.localPosition = localPosition;
		}
		else
		{
			startNewEffect();
		}
	}

	private void Update()
	{
		if (currentState == EffectState.Showing)
		{
			if (stateStartTime + showingDuration <= Time.time)
			{
				SetState(EffectState.Fading);
				fader.Activate();
			}
		}
		else if (currentState == EffectState.Fading)
		{
			float num = (Time.time - stateStartTime) / fadingDuration;
			float y = Mathf.Lerp(originalYPosition, originalYPosition + fadingSlideAmount, num);
			transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
			if (stateStartTime + startNewEffectDelay <= Time.time && !haveStartedNewEffect)
			{
				startNewEffect();
				haveStartedNewEffect = true;
			}
			if (num >= 1f)
			{
				SetState(EffectState.Inactive);
			}
		}
	}

	private void SetState(EffectState newState)
	{
		currentState = newState;
		stateStartTime = Time.time;
	}
}
