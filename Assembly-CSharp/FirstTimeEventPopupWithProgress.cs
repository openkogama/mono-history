using UnityEngine;
using UnityEngine.UI;

public class FirstTimeEventPopupWithProgress : FirstTimeEventPopup
{
	[SerializeField]
	private Scrollbar ProgressBar;

	[SerializeField]
	private Text progressText;

	private float progress;

	private float interpolationSpeed = 0.7f;

	private float interpolateToSize;

	private float Progress
	{
		get
		{
			return progress;
		}
		set
		{
			progress = Mathf.Clamp01(value);
			ProgressBar.size = progress;
		}
	}

	public void SetProgress(float current, float max)
	{
		progressText.text = current + " / " + max;
		interpolateToSize = current / max;
	}

	private void Update()
	{
		if (progress != interpolateToSize)
		{
			Progress = Mathf.Min(interpolateToSize, progress + Time.deltaTime * interpolationSpeed);
		}
	}
}
