public class TouristLoginQuit : QuitBaseCallback
{
	public void OnQuit()
	{
		BrowserComm.ToJavaScript.ExternalCall("gotoLogin");
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.loginURL);
	}
}
