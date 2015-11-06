public class QuitIdle : QuitBaseCallback
{
	public void OnQuit()
	{
		BrowserComm.ToJavaScript.ExternalCall("gotoIdlePage");
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.idleURL);
	}
}
