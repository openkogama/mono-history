public class ChatController
{
	protected MVGUIChatWindow chatWindow;

	public bool CanAutoHide
	{
		set
		{
			chatWindow.CanAutoHide = value;
		}
	}

	public ChatController()
	{
		chatWindow = UXUtils.FindGUIObjectOfType<MVGUIChatWindow>();
		chatWindow.InitializeChat();
	}

	public void ShowChat(bool takeControl, bool retainControlAfterMessageSend)
	{
		chatWindow.ShowChat(takeControl, retainControlAfterMessageSend);
	}

	public void HideChat()
	{
		chatWindow.HideChat();
	}
}
