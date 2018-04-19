using System.Collections;
using GameMeterVisuals;
using UnityEngine;

public class HealthbarLerp : GameMeterVisualEffect
{
	private float lerpForSeconds = 1f;

	private float lerpDelay = 0.4f;

	private float lerpStart;

	private float startProgress;

	private bool isInitialized;

	[SerializeField]
	private ProgressBar progressBar;

	[SerializeField]
	private ProgressBar targetProgressBar;

	private void OnEnable()
	{
		progressBar.Progress = targetProgressBar.Progress;
		isInitialized = true;
	}

	public override void ExecuteEffect()
	{
		if (gameObject.activeInHierarchy && isInitialized)
		{
			if (progressBar.Progress < targetProgressBar.Progress)
			{
				progressBar.Progress = targetProgressBar.Progress;
			}
			lerpStart = Time.realtimeSinceStartup;
			startProgress = progressBar.Progress;
			StopAllCoroutines();
			StartCoroutine(LerpProgress());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		progressBar.Progress = targetProgressBar.Progress;
	}

	private IEnumerator LerpProgress()
	{
		while (Time.realtimeSinceStartup - lerpStart < lerpDelay)
		{
			yield return 0;
		}
		for (float progress = 0f; progress < 1f; progress += Time.deltaTime / lerpForSeconds)
		{
			progressBar.Progress = Mathf.Lerp(startProgress, targetProgressBar.Progress, progress);
			if (progressBar.Progress <= targetProgressBar.Progress)
			{
				break;
			}
			yield return 0;
		}
		yield return 0;
	}
}
