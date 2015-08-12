public class MVGUIControlRoomButton : UXViewScript
{
	public UXIconButton controlRoomButton;

	public override void OnInitialize()
	{
		controlRoomButton.OnClick = () =>
		{
			UXUtils.UXDialogFactory.CreateCustomDialog("Prefabs/GUI/ControlRoom/ControlRoomDialog", TM._("Control Room")).AddPositiveButton(TM._("Save")).AddNegativeButton(TM._("Cancel"))
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
