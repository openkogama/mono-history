using UnityEngine;

public class GameBriefingText
{
	private enum BriefingState
	{
		Showing,
		Fading
	}

	private float startTime;

	private float duration;

	private float fadeDuration = 3.2f;

	private UXText uxText;

	private string text;

	private BriefingState state;

	public GameBriefingText(UXText uxTextEntity, float duration, string text)
	{
		startTime = Time.realtimeSinceStartup;
		uxText = uxTextEntity;
		uxText.text = text;
		this.duration = duration;
		this.text = text;
	}

	public bool Update()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (state == BriefingState.Showing)
		{
			uxText.Text = text;
			if (Time.realtimeSinceStartup - startTime > duration)
			{
				startTime = Time.realtimeSinceStartup;
				state = BriefingState.Fading;
			}
		}
		if (state == BriefingState.Fading)
		{
			float a = 1f - (Time.realtimeSinceStartup - startTime) / fadeDuration;
			Color color = uxText.Color;
			Color shadowColor = uxText.ShadowColor;
			color.a = a;
			shadowColor.a = a;
			uxText.Color = color;
			uxText.ShadowColor = shadowColor;
			if (Time.realtimeSinceStartup - startTime > fadeDuration)
			{
				uxText.Text = string.Empty;
				color.a = 1f;
				shadowColor.a = 1f;
				uxText.Color = color;
				uxText.ShadowColor = shadowColor;
				return false;
			}
		}
		return true;
	}

	public void Reset()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		uxText.Text = string.Empty;
		Color color = uxText.Color;
		Color shadowColor = uxText.ShadowColor;
		color.a = 1f;
		shadowColor.a = 1f;
		uxText.Color = color;
		uxText.ShadowColor = shadowColor;
	}
}
