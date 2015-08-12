using System;
using MV.WorldObject;
using UnityEngine;

public class MVGUIChangeTeamButton : MonoBehaviour
{
	public UXIconButton button;

	public void Initialize()
	{
		UXIconButton uXIconButton = button;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, new UXBaseButton.OnClickDelegate(ShowTeamSelectDialog));
	}

	public void ShowTeamSelectDialog()
	{
		if (MVGameController.Game.TeamManager.TeamCount() > 1)
		{
			UXUtils.UXDialogFactory.CreateCustomDialog("Prefabs/GUI/TeamSelect/TeamSelectDialog", string.Empty, noButtons: true, stackDialog: false, canClose: false).SetOnResultCallback(TeamSelectCallBack).Show();
		}
		else
		{
			UXUtils.UXDialogFactory.CreateDialog(TM._("Only one team in world."), TM._("Change Team")).Show();
		}
	}

	private void TeamSelectCallBack(UXDialogBox dialog)
	{
		MVTeam team = (MVTeam)(int)dialog.GetResult();
		MVGameController.Game.SetTeam(team);
	}
}
