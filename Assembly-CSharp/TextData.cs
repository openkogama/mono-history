using UnityEngine;

public class TextData : DialogData
{
	public string text;

	public bool useWordWrap;

	public override void ApplyDataToElement(GameObject element)
	{
		UXText component = element.GetComponent<UXText>();
		if (!(component == null))
		{
			component.WordWrap = useWordWrap;
			component.Text = text;
		}
	}
}
