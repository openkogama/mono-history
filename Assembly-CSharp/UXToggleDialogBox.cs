public class UXToggleDialogBox : UXDialogBox
{
	public override object GetResult()
	{
		return GetComponentInChildren<UXToggleIconButton>().ToggleState;
	}
}
