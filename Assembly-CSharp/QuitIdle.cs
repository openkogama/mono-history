public class QuitIdle : QuitBaseCallback
{
	public void OnQuit()
	{
		BrowserCommGotoRequests.GotoIdle();
	}
}
