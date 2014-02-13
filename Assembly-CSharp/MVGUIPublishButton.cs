using Localize;

public class MVGUIPublishButton : UXViewScript
{
	public UXIconButton publishButton;

	public override void OnInitialize()
	{
		publishButton.OnClick = () =>
		{
			if (MVGameController.Instance.Game.LocalPlayer.PlanetOwnershipTypeID == 2)
			{
				UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateCustomDialog("Prefabs/GUI/Dialogs/PublishDialog", TextSlotIndex.PublishPlanet).AddPositiveButton(TextSlotIndex.Confirm)
					.AddNegativeButton(TextSlotIndex.Reject)
					.SetOnResultCallback(PublishButtonOnClick)
					.Show();
			}
			else
			{
				UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.PlanetAuthorizeError, TextSlotIndex.ErrorHeadline).Show();
			}
		};
	}

	private void PublishButtonOnClick(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			MVGameController.Instance.Game.PublishPlanet();
		}
	}
}
