using System;
using System.Collections.Generic;
using UnityEngine;

public class FadeTransition : MonoBehaviour
{
	[SerializeField]
	private GuiFadeState testFadeState;

	[SerializeField]
	private float fadeInTime;

	[SerializeField]
	private float fadeOutTime;

	[SerializeField]
	private List<FadeTransitionBaseAlpha> fadeTransitionBaseAlphas;

	private float alpha;

	private float alphaChangePrSec;

	private Action doneCallback;

	[SerializeField]
	private UXGUIElement[] uxElements;

	public GuiFadeState GuiFadeState { get; private set; }

	private void Start()
	{
		if (uxElements == null || uxElements.Length == 0)
		{
			Debug.Log("Get components in children");
			uxElements = GetComponentsInChildren<UXGUIElement>(includeInactive: true);
		}
	}

	private void Update()
	{
		HandleTest();
		alpha += Time.deltaTime * alphaChangePrSec;
		alpha = Mathf.Clamp01(alpha);
		UXGUIElement[] array = uxElements;
		foreach (UXGUIElement uXGUIElement in array)
		{
			float num = 1f;
			foreach (FadeTransitionBaseAlpha fadeTransitionBaseAlpha in fadeTransitionBaseAlphas)
			{
				if (fadeTransitionBaseAlpha.gameObject.GetInstanceID() == uXGUIElement.gameObject.GetInstanceID())
				{
					num = fadeTransitionBaseAlpha.baseAlpha;
				}
			}
			uXGUIElement.SetAlpha(alpha * num, string.Empty);
		}
		if (alpha == 0f || alpha == 1f)
		{
			if (doneCallback != null)
			{
				doneCallback();
				doneCallback = null;
			}
			GuiFadeState = GuiFadeState.None;
		}
	}

	private void HandleTest()
	{
		if (testFadeState == GuiFadeState.FadeIn)
		{
			FadeIn(() =>
			{
				Debug.Log("Faded in done");
			});
		}
		if (testFadeState == GuiFadeState.FadeOut)
		{
			FadeOut(() =>
			{
				Debug.Log("Faded out done");
			});
		}
		testFadeState = GuiFadeState.None;
	}

	public void FadeIn(Action callback)
	{
		SetFadeState(fadeInTime, callback, 1f, GuiFadeState.FadeIn);
	}

	public void FadeOut(Action callback)
	{
		SetFadeState(fadeOutTime, callback, -1f, GuiFadeState.FadeOut);
	}

	private void SetFadeState(float nextFadeTime, Action callback, float direction, GuiFadeState guiFadeState)
	{
		doneCallback = callback;
		alphaChangePrSec = 1f / nextFadeTime * direction;
		GuiFadeState = guiFadeState;
	}
}
