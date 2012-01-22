using UnityEngine;

public class MVGUIChatToggle : UXViewScript
{
	public UXIconButton chat;

	public override void OnShow()
	{
		((Component)chat).gameObject.SetActiveRecursively(true);
	}

	public override void OnHide()
	{
		((Component)chat).gameObject.SetActiveRecursively(false);
	}
}
