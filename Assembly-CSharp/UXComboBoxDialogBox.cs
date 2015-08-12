public class UXComboBoxDialogBox : UXDialogBox
{
	public override void OnShowDialog()
	{
		base.OnShowDialog();
		GetComponentInChildren<UXComboBox>().Close();
	}

	public override object GetResult()
	{
		UXComboBoxItem currentlySelectedItem = GetComponentInChildren<UXComboBox>().CurrentlySelectedItem;
		if (currentlySelectedItem == null)
		{
			return string.Empty;
		}
		return currentlySelectedItem.GetValue();
	}
}
