using System;
using UnityEngine;

public class NotificationFade : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup group;

	[SerializeField]
	private AnimationCurve textVisibilityCurve;

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private bool playing = true;

	private float currentTime;

	public Action OnFinished;

	public void Deactivate()
	{
		if (playing)
		{
			playing = false;
			group.alpha = 0f;
			currentTime = 0f;
			if (OnFinished != null)
			{
				OnFinished();
			}
		}
	}

	public void Activate()
	{
		playing = true;
		group.alpha = 0f;
		currentTime = 0f;
	}

	private void Update()
	{
		if (playing)
		{
			currentTime += Time.deltaTime;
			group.alpha = textVisibilityCurve.Evaluate(currentTime / duration);
			if (currentTime >= duration)
			{
				Deactivate();
			}
		}
	}

	private void OnDisable()
	{
		Deactivate();
	}
}
