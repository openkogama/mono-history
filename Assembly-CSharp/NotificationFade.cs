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

	private float currentTime;

	public Action OnFinished;

	private bool finished;

	public void Deactivate()
	{
		if (!finished)
		{
			finished = true;
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
		finished = false;
		group.alpha = 0f;
		currentTime = 0f;
	}

	private void Update()
	{
		if (!finished)
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
