using UnityEngine;

public class TextFieldData : DialogData
{
	public string text;

	public TextInputType? allowedInput;

	public override void ApplyDataToElement(GameObject element)
	{
		UXTextField component = element.GetComponent<UXTextField>();
		if (!(component == null))
		{
			component.Text = text;
			TextInputType? textInputType = allowedInput;
			if (textInputType.HasValue)
			{
				TextInputType? textInputType2 = allowedInput;
				component.allowedInput = textInputType2.Value;
			}
		}
	}
}
