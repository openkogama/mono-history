public class UXTextFieldDialogBox : UXDialogBox
{
	public override void OnCloseDialog()
	{
		base.OnCloseDialog();
		(UXUtils.FindComponentInParents(typeof(UXView), transform.parent) as UXView).ReleaseFocus();
	}

	public override object GetResult()
	{
		return GetComponentInChildren<UXTextField>().Text;
	}
}
