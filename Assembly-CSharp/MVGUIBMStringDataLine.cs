using System.Collections.Generic;

public class MVGUIBMStringDataLine : MVGUIBMDataLine
{
	private string value;

	public override void BuildLine(string name, object value)
	{
		base.BuildLine(name, value);
		this.value = (string)value;
		typeText.Text = "S";
		valueText.Text = this.value;
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
			allowedInput = TextInputType.All
		});
		UXUtils.UXDialogFactory.CreateDialog("String Value", "Change Value", UXDialogType.TextField, noButtons: false, stackDialog: true).SetValues(dictionary).SetOnResultCallback(OnModifyResult)
			.Show();
	}

	private void OnModifyResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			value = (string)dialogBox.GetResult();
			valueText.Text = value;
		}
	}
}
