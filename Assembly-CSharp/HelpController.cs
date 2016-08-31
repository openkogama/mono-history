using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpController : MonoBehaviour
{
	[SerializeField]
	private HelpPopup helpScreen;

	private HelpPopup popup;

	private bool fileNotFound;

	private string path = "LocalizedGraphics/{0}/helpScreen.png";

	public void OnClick()
	{
		popup = Object.Instantiate(helpScreen);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
		AsyncWWWManager.WWWRequest(new CachedGetRequest(string.Format(Urls.StreamingAssets + path, TM.CultureName), StreamingAssetLoaded, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	private void StreamingAssetLoaded(WWW www)
	{
		if (www == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			if (!fileNotFound)
			{
				fileNotFound = true;
				AsyncWWWManager.WWWRequest(new CachedGetRequest(string.Format(Urls.StreamingAssets + path, "en-US"), StreamingAssetLoaded, WWWRequestPriority.WaitUntilSyncronizingIsDone));
			}
		}
		else if (www.texture != null)
		{
			popup.Initialize(www.texture);
		}
	}
}
