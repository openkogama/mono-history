using System;
using UnityEngine;

public class FirstTimeFadeHandler : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private float fadeOutTime = 0.3f;

	private float currentFadeOutTime;

	[SerializeField]
	private float fadeInTime = 0.3f;

	private float currentFadeInTime;

	private bool fadingIn;

	private bool fadingOut;

	private Action<GameObject> finishedAction;

	private GameObject targetGameObject;

	public void StartFadeIn()
	{
		fadingIn = true;
		canvasGroup.alpha = 0f;
		currentFadeInTime = 0f;
	}

	public void StartFadeOut(Action<GameObject> finishedAction, GameObject targetGameObject)
	{
		currentFadeOutTime = fadeOutTime;
		fadingOut = true;
		canvasGroup.alpha = 1f;
		this.finishedAction = (Action<GameObject>)Delegate.Combine(this.finishedAction, finishedAction);
		this.targetGameObject = targetGameObject;
	}

	private void Update()
	{
		if (fadingOut)
		{
			currentFadeOutTime -= Time.deltaTime;
			canvasGroup.alpha = currentFadeOutTime / fadeOutTime;
			if (currentFadeOutTime <= 0f)
			{
				fadingOut = false;
				DoAction();
			}
		}
		else if (fadingIn)
		{
			currentFadeInTime += Time.deltaTime;
			canvasGroup.alpha = currentFadeInTime / fadeInTime;
			if (currentFadeInTime >= fadeInTime)
			{
				fadingIn = false;
			}
		}
	}

	private void DoAction()
	{
		if (finishedAction != null)
		{
			finishedAction(targetGameObject);
		}
		finishedAction = null;
		targetGameObject = null;
	}
}
