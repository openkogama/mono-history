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

	private UnityAction<bool> resultCallback;

	public void Initialize(UnityAction<bool> resultCallback, Texture previewImage, AccessoryDataClient accessoryData)
	{
		preview.texture = previewImage;
		this.resultCallback = resultCallback;
		itemBackground.Initialize(accessoryData);
	}

	public void Equip()
	{
		resultCallback(arg0: true);
	}

	public void DontEquip()
	{
		resultCallback(arg0: false);
	}
}
