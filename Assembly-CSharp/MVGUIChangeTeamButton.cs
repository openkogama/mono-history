using System;
using UnityEngine;

public class MVGUIChangeTeamButton : MonoBehaviour
{
	public UXIconButton button;

	public void Initialize()
	{
		UXIconButton uXIconButton = button;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, new UXBaseButton.OnClickDelegate(MVGameControllerLegacyUI.PlayController.ShowTeamSelectDialog));
	}
}
