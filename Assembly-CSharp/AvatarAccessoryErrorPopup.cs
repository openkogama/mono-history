using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class AvatarAccessoryErrorPopup : MonoBehaviour
{
	[SerializeField]
	private RawImage preview;

	[SerializeField]
	private AccessoryItemBackground itemBackground;

	[SerializeField]
	private Text header;

	[SerializeField]
	private Text buttonText;

	private UnityAction<bool> resultCallback;

	public void Initialize(UnityAction<bool> resultCallback, Texture previewImage, AccessoryDataClient accessoryData, string header, string buttonText)
	{
		this.buttonText.text = buttonText;
		this.header.text = header;
		preview.texture = previewImage;
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
}
