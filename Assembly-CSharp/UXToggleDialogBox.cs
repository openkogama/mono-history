using UnityEngine;

public class UXToggleDialogBox : UXDialogBox
{
	public override object GetResult()
	{
		return ((Component)this).GetComponentInChildren<UXToggleIconButton>().ToggleState;
	}
}
