using UnityEngine;

public class QuitBrowserRequest : QuitBaseCallback
{
	private readonly string url;

	public QuitBrowserRequest(string url)
	{
		this.url = url;
	}

	public void OnQuit()
	{
		Debug.Log("QuitBrowserRequest");
		BrowserComm.ExecuteBrowserRequest(url);
	}
}
