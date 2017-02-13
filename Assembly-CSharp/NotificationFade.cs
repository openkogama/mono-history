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

	private void OnEnable()
	{
		group.alpha = 0f;
		currentTime = 0f;
	}

	private void Update()
	{
		currentTime += Time.deltaTime;
		group.alpha = textVisibilityCurve.Evaluate(currentTime / duration);
		if (currentTime >= duration)
		{
			group.alpha = 0f;
		}
	}
}
