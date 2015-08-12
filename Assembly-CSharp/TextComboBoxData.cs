using UnityEngine;

public class TextComboBoxData : DialogData
{
	public string[] items;

	public int currentlySelectedIndex;

	public override void ApplyDataToElement(GameObject element)
	{
		UXComboBox component = element.GetComponent<UXComboBox>();
		if (!(component == null))
		{
			component.Add(items);
			component.SetCurrentItem(currentlySelectedIndex);
		}
	}
}
