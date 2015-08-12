public class MVGUIPublishButton : UXViewScript
{
	public UXIconButton publishButton;

	public override void OnInitialize()
	{
		publishButton.OnClick = () =>
		{
			if (MVGameController.Game.LocalPlayer.PlanetOwnershipTypeID == 2)
			{
				UXUtils.UXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/PublishDialog", TM._("Publish Your Game")).AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("No"))
					.SetOnResultCallback(PublishButtonOnClick)
					.Show();
			}
			else
			{
				UXUtils.UXDialogFactory.CreateDialog(TM._("You are not authorized to\npublish this planet"), TM._("Error")).Show();
			}
		};
	}

	private void PublishButtonOnClick(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			MVGameController.Game.PublishPlanet();
		}
	}
}
