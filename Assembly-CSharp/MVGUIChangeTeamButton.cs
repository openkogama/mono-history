using System;
using Localize;
using MV.WorldObject;
using UnityEngine;

public class MVGUIChangeTeamButton : MonoBehaviour
{
	public void Initialize()
	{
		UXIconButton component = ((Component)this).GetComponent<UXIconButton>();
		component.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(component.OnClick, new UXBaseButton.OnClickDelegate(ShowTeamSelectDialog));
	}

	public void ShowTeamSelectDialog()
	{
		if (MVGameController.Instance.Game.TeamManager.TeamCount() > 1)
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateCustomDialog("Prefabs/GUI/TeamSelect/TeamSelectDialog", TextSlotIndex.Empty, noButtons: true, stackDialog: false, canClose: false).SetOnResultCallback(TeamSelectCallBack)
				.Show();
			return;
		}
		MVGameController.Instance.Game.SetTeam(MVTeam.None);
		UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.OneTeamWarning, TextSlotIndex.ChangeTeamMessage).Show();
	}

	private void TeamSelectCallBack(UXDialogBox dialog)
	{
		MVTeam team = (MVTeam)(int)dialog.GetResult();
		MVGameController.Instance.Game.SetTeam(team);
		if (MVGameController.Instance.PlayController != null)
		{
			MVGameController.Instance.WOCM.AvatarLocal.Respawn(suicide: true);
		}
	}
}
