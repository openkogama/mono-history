using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundViewItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private bool previewLoaded;

	private SoundTabInfo tabInfo;

	[SerializeField]
	private ToolTip toolTip;

	[SerializeField]
	private Image selectedImage;

	[SerializeField]
	private Image soundImage;

	[SerializeField]
	private Text title;

	[SerializeField]
	private GameObject loadingWheel;

	[SerializeField]
	private Button buttonComponent;

	private UnityAction<string> setNewOriginalUrl;

	private string originalUrl;

	private bool doneLoading;

	public void Initialize(SoundTabInfo info, string originalUrl, UnityAction<string> setNewOriginalUrl)
	{
		this.setNewOriginalUrl = setNewOriginalUrl;
		this.originalUrl = originalUrl;
		tabInfo = info;
		loadingWheel.SetActive(value: true);
		soundImage.gameObject.SetActive(value: false);
		title.text = TM._("Loading...");
		toolTip.SetText(TM._("Loading..."));
		if (tabInfo.url == originalUrl)
		{
			selectedImage.gameObject.SetActive(value: true);
		}
		string path = StreamingAsset.DBUrlToServerUrl(StreamingAsset.AssetBundleUrl + tabInfo.url);
		AsyncWWWManager.WWWRequest(new CachedGetRequest(path, OnDownloadFinished, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	private void OnDownloadFinished(WWW www)
	{
		if (www == null)
		{
			Debug.LogWarning("Error loading sound, download unable to be finished.");
			title.text = TM._("Download failed");
			toolTip.SetText(TM._("Sound failed to download."));
			return;
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogWarning("error from www in SoundViewItem! " + www.error);
			title.text = TM._("Download failed");
			toolTip.SetText(TM._("Sound failed to download."));
			return;
		}
		buttonComponent.interactable = true;
		loadingWheel.SetActive(value: false);
		soundImage.gameObject.SetActive(value: true);
		toolTip.SetText(tabInfo.name);
		title.text = tabInfo.name;
		doneLoading = true;
	}

	public void UnsubscribePendingDownloads()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (tabInfo != null && doneLoading)
		{
			ChangeUrl(tabInfo.url);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ChangeUrl(originalUrl);
	}

	public void OnClick()
	{
		if (tabInfo != null && doneLoading)
		{
			originalUrl = tabInfo.url;
			setNewOriginalUrl(tabInfo.url);
			ChangeUrl(tabInfo.url);
		}
	}

	private void ChangeUrl(string url)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleSettingChanged handler, BaseEventData data) =>
		{
			handler.OnSettingChanged("url", url);
		});
	}

	private void OnDestroy()
	{
		UnsubscribePendingDownloads();
	}
}
