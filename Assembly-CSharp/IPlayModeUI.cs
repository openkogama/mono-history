public interface IPlayModeUI
{
	bool InLobbyState { get; set; }

	void ShowEUseIcon(ShowUseOption option, int woId = 0);

	void HideEUseIcon();

	IGUICrossHair GetCrossHair();
}
