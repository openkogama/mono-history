public interface IPlayModeUI
{
	bool InLobbyState { get; set; }

	void ShowEUseIcon(ShowUseOption option);

	void HideEUseIcon();

	IGUICrossHair GetCrossHair();
}
