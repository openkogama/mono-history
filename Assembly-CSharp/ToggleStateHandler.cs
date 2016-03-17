using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleStateHandler : ToggleStatHandlerBase
{
	[SerializeField]
	private Sprite toggleOn;

	[SerializeField]
	private Sprite toggleOff;

	protected override void UpdateToggleState()
	{
		if (toggleState)
		{
			button.image.sprite = toggleOn;
		}
		else
		{
			button.image.sprite = toggleOff;
		}
	}
}
