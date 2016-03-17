using UnityEngine;

public class ToggleStateHandlerTransparent : ToggleStatHandlerBase
{
	[SerializeField]
	private float toggled = 1f;

	[SerializeField]
	private float notToggled = 0.3f;

	protected override void UpdateToggleState()
	{
		Color color = button.image.color;
		if (toggleState)
		{
			color.a = toggled;
		}
		else
		{
			color.a = notToggled;
		}
		button.image.color = color;
	}
}
