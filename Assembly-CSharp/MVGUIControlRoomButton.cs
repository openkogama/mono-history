using Localize;

public class MVGUIControlRoomButton : UXViewScript
{
	public UXIconButton controlRoomButton;

	public override void OnInitialize()
	{
		controlRoomButton.OnClick = () =>
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateCustomDialog("Prefabs/GUI/ControlRoom/ControlRoomDialog", TextSlotIndex.ControlRoom).AddPositiveButton(TextSlotIndex.Save)
				.AddNegativeButton(TextSlotIndex.Cancel)
				.SetOnResultCallback(ControlRoomOnResult)
				.Show();
		};
	}

	private void ControlRoomOnResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult != UXDialogResult.Positive)
		{
		}
	}
}
