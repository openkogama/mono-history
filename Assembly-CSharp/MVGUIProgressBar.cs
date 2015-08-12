using UnityEngine;

public class MVGUIProgressBar : UXGUIElement
{
	private float targetPercentage;

	private float currentPercentage;

	[SerializeField]
	private float progressBarSpeed = 2f;

	[SerializeField]
	private UXPlane progressCurrent;

	[SerializeField]
	private UXPlane progressBackground;

	public float Percentage
	{
		get
		{
			return targetPercentage;
		}
		set
		{
			targetPercentage = value;
		}
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		progressCurrent.SetVisible(visible);
		progressBackground.SetVisible(visible);
	}

	private void UpdateProgressBarPercentage()
	{
		if (currentPercentage > targetPercentage)
		{
			currentPercentage = targetPercentage;
		}
		if (progressBarSpeed == 0f)
		{
			currentPercentage = targetPercentage;
		}
		else
		{
			currentPercentage = Mathf.Lerp(currentPercentage, targetPercentage, Time.deltaTime * progressBarSpeed);
		}
		Vector3 localScale = progressCurrent.transform.localScale;
		localScale.x = currentPercentage;
		progressCurrent.transform.localScale = localScale;
	}

	public override void Update()
	{
		base.Update();
		UpdateProgressBarPercentage();
	}
}
