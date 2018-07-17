using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class AvatarAccessoryErrorPopup : MonoBehaviour
{
	[SerializeField]
	private StreamPngToSprite preview;

	[SerializeField]
	private AccessoryItemBackground itemBackground;

	[SerializeField]
	private Text header;

	[SerializeField]
	private Text buttonText;

	[SerializeField]
	private GameObject loadingWheel;

	[SerializeField]
	private GameObject emptyFrame;

	private UnityAction<bool> resultCallback;

	public void Initialize(UnityAction<bool> resultCallback, string previewImageUrl, AccessoryDataClient accessoryData, string header, string buttonText)
	{
		this.buttonText.text = buttonText;
		this.header.text = header;
		loadingWheel.SetActive(value: true);
		preview.gameObject.SetActive(value: false);
		StreamPngToSprite streamPngToSprite = preview;
		streamPngToSprite.OnDownloadFinish = (Action)Delegate.Combine(streamPngToSprite.OnDownloadFinish, new Action(OnPreviewImageDownLoaded));
		preview.StartDownloading(previewImageUrl);
		this.resultCallback = resultCallback;
		itemBackground.Initialize(accessoryData);
	}

	public void OnButtonPressed(bool confirmed)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		resultCallback(confirmed);
	}

	private void OnPreviewImageDownLoaded()
	{
		loadingWheel.SetActive(value: false);
		emptyFrame.SetActive(value: false);
		preview.gameObject.SetActive(value: true);
	}

	private void OnDestroy()
	{
		StreamPngToSprite streamPngToSprite = preview;
		streamPngToSprite.OnDownloadFinish = (Action)Delegate.Remove(streamPngToSprite.OnDownloadFinish, new Action(OnPreviewImageDownLoaded));
		preview.DestroyTexture();
	}
}
