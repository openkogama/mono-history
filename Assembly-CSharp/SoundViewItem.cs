using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundViewItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private bool previewLoaded;

	private StreamingAssetInfo info;

	[SerializeField]
	private ToolTip toolTip;

	[SerializeField]
	private Image selectedImage;

	[SerializeField]
	private Text title;

	private UnityAction<string> setNewOriginalUrl;

	private string originalUrl;

	public void Initialize(StreamingAssetInfo info, string originalUrl, UnityAction<string> setNewOriginalUrl)
	{
		this.setNewOriginalUrl = setNewOriginalUrl;
		this.originalUrl = originalUrl;
		this.info = info;
		title.text = info.Name;
		toolTip.SetText(info.Name);
		if (info.AssetPath == originalUrl)
		{
			selectedImage.gameObject.SetActive(value: true);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ChangeUrl(info.AssetPath);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ChangeUrl(originalUrl);
	}

	public void OnClick()
	{
		originalUrl = info.AssetPath;
		setNewOriginalUrl(info.AssetPath);
		ChangeUrl(info.AssetPath);
	}

	private void ChangeUrl(string url)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleSettingChanged handler, BaseEventData data) =>
		{
			handler.OnSettingChanged("url", url);
		});
	}
}
