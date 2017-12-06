using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundViewItem : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
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
		if (www != null && !string.IsNullOrEmpty(www.error) && www.error.Length > 0)
		{
			Debug.LogWarning("error from www in SoundViewItem! " + www.error);
			return;
		}
		loadingWheel.SetActive(value: false);
		soundImage.gameObject.SetActive(value: true);
		toolTip.SetText(tabInfo.name);
		title.text = tabInfo.name;
		doneLoading = true;
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
		AsyncWWWManager.UnsubscribeWWWRequest(OnDownloadFinished);
	}
}
