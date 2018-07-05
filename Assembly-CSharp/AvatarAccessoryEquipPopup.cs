using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class AvatarAccessoryEquipPopup : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private RawImage preview;

	[SerializeField]
	private AccessoryItemBackground itemBackground;

	private UnityAction resultCallback;

	private AccessoryDataClient accessoryDataClient;

	private MVBody avatarBody;

	public void Initialize(UnityAction resultCallback, Texture previewImage, AccessoryDataClient accessoryData, MVBody avatarBody)
	{
		preview.texture = previewImage;
		this.resultCallback = resultCallback;
		itemBackground.Initialize(accessoryData);
		accessoryDataClient = accessoryData;
		this.avatarBody = avatarBody;
	}

	public void Equip()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAttachToBody x, BaseEventData y) =>
		{
			x.AttachToBody(accessoryDataClient.streamingAssetID, avatarBody.GetAccessoryOffset(accessoryDataClient.accessorySlotType), avatarBody.GetAccessoryScale(accessoryDataClient.accessorySlotType));
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		resultCallback();
	}

	public void DontEquip()
	{
		resultCallback();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
