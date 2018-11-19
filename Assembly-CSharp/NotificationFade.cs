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

	private float pauseAt = 1f;

	public Action OnFinished;

	public bool IsPaused => pauseAt == duration;

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
		pauseAt = duration;
		group.alpha = 0f;
		currentTime = 0f;
	}

	public void PauseAt(float pausePoint)
	{
		pauseAt = pausePoint;
	}

	public void Unpause()
	{
		if (!IsPaused)
		{
			currentTime = pauseAt;
			pauseAt = duration;
		}
	}

	private void Update()
	{
		if (playing)
		{
			currentTime += Time.deltaTime;
			if (currentTime > pauseAt)
			{
				currentTime = pauseAt;
			}
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
