using UnityEngine;

public class UXComboBoxDialogBox : UXDialogBox
{
	public override void OnShowDialog()
	{
		base.OnShowDialog();
		((Component)this).GetComponentInChildren<UXComboBox>().Close();
	}

	public override object GetResult()
	{
		UXComboBoxItem currentlySelectedItem = ((Component)this).GetComponentInChildren<UXComboBox>().CurrentlySelectedItem;
		if ((Object)(object)currentlySelectedItem == (Object)null)
		{
			return string.Empty;
		}
		return currentlySelectedItem.GetValue();
	}
}
