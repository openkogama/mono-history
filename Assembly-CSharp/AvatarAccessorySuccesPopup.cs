using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class AvatarAccessorySuccesPopup : MonoBehaviour
{
	[SerializeField]
	private RawImage preview;

	[SerializeField]
	private AccessoryItemBackground itemBackground;

	private UnityAction resultCallback;

	public void Initialize(UnityAction resultCallback, Texture previewImage, AccessoryDataClient accessoryData)
	{
		preview.texture = previewImage;
		this.resultCallback = resultCallback;
		itemBackground.Initialize(accessoryData);
	}

	public void OnButtonPressed()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		resultCallback();
	}
}
