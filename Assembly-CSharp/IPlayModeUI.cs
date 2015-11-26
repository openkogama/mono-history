public interface IPlayModeUI
{
	bool InLobbyState { get; set; }

	void ShowEUseIcon(ShowUseOption option, int level = 0);

	void HideEUseIcon();

	IGUICrossHair GetCrossHair();
}
