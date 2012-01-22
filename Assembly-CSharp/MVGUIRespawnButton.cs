public class MVGUIRespawnButton : UXViewScript
{
	public UXIconButton respawn;

	public override void OnInitialize()
	{
		respawn.OnClick = () =>
		{
			MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.Respawn();
		};
	}
}
