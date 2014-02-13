using UnityEngine;

public class ToggleIconData : DialogData
{
	public bool toggleValue;

	public override void ApplyDataToElement(GameObject element)
	{
		UXToggleIconButton component = element.GetComponent<UXToggleIconButton>();
		if (!((Object)(object)component == (Object)null))
		{
			component.ToggleState = toggleValue;
		}
	}
}
