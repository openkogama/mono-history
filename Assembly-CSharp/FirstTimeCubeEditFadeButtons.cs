using UnityEngine;

public class FirstTimeCubeEditFadeButtons : MonoBehaviour
{
	[SerializeField]
	private float fadeIn = 0.2f;

	[SerializeField]
	private CanvasGroup canvasGroup;

	private float currentTime;

	private bool doFading;

	private void Awake()
	{
		canvasGroup.interactable = false;
		canvasGroup.alpha = 0f;
	}

	public void ActivateImmediate()
	{
		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
	}

	public bool IsEnabled()
	{
		return canvasGroup.interactable || doFading;
	}

	public void Activate()
	{
		currentTime = 0f;
		doFading = true;
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
	}

	private void Update()
	{
		if (doFading)
		{
			currentTime += Time.deltaTime;
			canvasGroup.alpha = Mathf.Clamp01(currentTime / fadeIn);
			if (currentTime >= fadeIn)
			{
				doFading = false;
				canvasGroup.alpha = 1f;
				canvasGroup.interactable = true;
			}
		}
	}
}
