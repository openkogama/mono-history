using System.Collections.Generic;
using Localize;

public class MVGUIBMIntDataLine : MVGUIBMDataLine
{
	private int value;

	public override void BuildLine(string name, object value)
	{
		base.BuildLine(name, value);
		this.value = (int)value;
		typeText.Text = "I";
		valueText.Text = string.Empty + this.value;
	}

	public override object GetValue()
	{
		return value;
	}

	protected override void ModifyLine()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("TextField", new TextFieldData
		{
			text = value + string.Empty,
			allowedInput = TextInputType.Numerical
		});
		UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.IntValue, TextSlotIndex.ChangeValue, UXDialogType.TextField, noButtons: false, stackDialog: true).SetValues(dictionary)
			.SetOnResultCallback(OnModifyResult)
			.Show();
	}

	private void OnModifyResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			int result = 0;
			if (int.TryParse((string)dialogBox.GetResult(), out result))
			{
				value = result;
				valueText.Text = string.Empty + value;
			}
			else
			{
				UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.FailedToParseValue, TextSlotIndex.ErrorHeadline).Show();
			}
		}
	}
}
