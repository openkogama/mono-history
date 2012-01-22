using System;

public class MVGUISocialWindow : UXViewScript
{
	public UXText chatText;

	public UXText playersText;

	private string chatLog = string.Empty;

	public void InitializeListeners()
	{
		MVNetworkGame game = MVGameController.Instance.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(OnPlayerListChanged));
		MVNetworkGame game2 = MVGameController.Instance.Game;
		game2.OnReceivedChatMessage = (MVNetworkGame.OnReceivedChatMessageDelegate)Delegate.Combine(game2.OnReceivedChatMessage, new MVNetworkGame.OnReceivedChatMessageDelegate(OnChatMessage));
	}

	public override void OnShow()
	{
		base.OnShow();
		OnPlayerListChanged();
	}

	private void OnPlayerListChanged()
	{
		string text = string.Empty;
		foreach (MVPlayer value in MVGameController.Instance.WOCM.Players.Values)
		{
			text = text + value.Username + "\n";
		}
		playersText.Text = text;
	}

	private void OnChatMessage(string sender, string message)
	{
		string text = chatLog;
		chatLog = text + sender + ": " + message + "\n";
		chatText.Text = chatLog;
	}
}
