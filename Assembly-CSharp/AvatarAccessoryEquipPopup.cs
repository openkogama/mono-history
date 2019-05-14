using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AvatarAccessoryEquipPopup : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private StreamedSpriteToImageManual preview;

	[SerializeField]
	private AccessoryItemBackground itemBackground;

	[SerializeField]
	private GameObject loadingWheel;

	[SerializeField]
	private GameObject emptyFrame;

	private UnityAction resultCallback;

	private AccessoryDataClient accessoryDataClient;

	private float accessoryOffset;

	private float accessoryScale;

	public void Initialize(UnityAction resultCallback, string previewImageUrl, AccessoryDataClient accessoryData, float accessoryOffset, float accessoryScale)
	{
		loadingWheel.SetActive(value: true);
		preview.gameObject.SetActive(value: false);
		preview.Download(previewImageUrl, OnPreviewImageDownLoaded);
		this.resultCallback = resultCallback;
		itemBackground.Initialize(accessoryData);
		accessoryDataClient = accessoryData;
		this.accessoryOffset = accessoryOffset;
		this.accessoryScale = accessoryScale;
	}

	public void Equip()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAttachToBody x, BaseEventData y) =>
		{
			x.AttachToBody(accessoryDataClient.sAID, accessoryOffset, accessoryScale);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		resultCallback();
	}

	public void DontEquip()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		resultCallback();
	}

	private void OnPreviewImageDownLoaded()
	{
		loadingWheel.SetActive(value: false);
		emptyFrame.SetActive(value: false);
		preview.gameObject.SetActive(value: true);
	}
}
