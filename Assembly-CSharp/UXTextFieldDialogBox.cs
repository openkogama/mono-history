using UnityEngine;

public class UXTextFieldDialogBox : UXDialogBox
{
	public override void OnCloseDialog()
	{
		base.OnCloseDialog();
		(UXUtils.FindComponentInParents(typeof(UXView), ((Component)this).transform.parent) as UXView).ReleaseFocus();
	}

	public override object GetResult()
	{
		return ((Component)this).GetComponentInChildren<UXTextField>().Text;
	}
}
