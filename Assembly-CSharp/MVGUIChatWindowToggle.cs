using System;

public class MVGUIChatWindowToggle : UXViewScript
{
	public UXIconButton chatIcon;

	private AIngameController Controller => MVGameController.Instance.IngameController;

	public override void OnInitialize()
	{
		UXIconButton uXIconButton = chatIcon;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, new UXBaseButton.OnClickDelegate(ToggleSocial));
	}

	public void ToggleSocial()
	{
		if (!MVGameController.Instance.Game.TouristChatAllowed && MVGameController.Instance.IsTouristSession)
		{
			MVGameController.Instance.IngameController.ShowRegisterPopup();
		}
		else if (Controller.IsChatShown())
		{
			Controller.HideChat();
		}
		else
		{
			Controller.ShowChat(fromShortcut: false);
		}
	}
}
