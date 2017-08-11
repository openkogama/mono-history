using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaterialViewItem : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	private byte id;

	private bool locked;

	private bool isAvailable = true;

	[SerializeField]
	private Image lockedImage;

	[SerializeField]
	private RawImage buttonImage;

	[SerializeField]
	private MaterialPurchasePopup materialPurchasePopupPrefab;

	[SerializeField]
	private float unavailableAlpha = 0.1f;

	[SerializeField]
	private Image selectedBackground;

	[SerializeField]
	private ToolTip toolTip;

	[SerializeField]
	private GameObject mouseHoverDescriptionFrame;

	public void Initialize(byte id, bool locked, Texture2D texture2D, bool isAvailable, bool isSelected)
	{
		toolTip.SetText(MaterialDescription.materialDescriptions[id].Name);
		this.isAvailable = isAvailable;
		this.id = id;
		this.locked = locked;
		lockedImage.gameObject.SetActive(locked);
		buttonImage.texture = texture2D;
		if (!isAvailable)
		{
			Color color = buttonImage.color;
			color.a = unavailableAlpha;
			buttonImage.color = color;
		}
		if (isSelected)
		{
			selectedBackground.gameObject.SetActive(value: true);
		}
	}

	public void OnClick()
	{
		if (locked)
		{
			MaterialPurchasePopup materialPurchasePopup = Object.Instantiate(materialPurchasePopupPrefab);
			materialPurchasePopup.Initialize(id, PurchaseCallback);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.PopGroups(UIGroupFlags.InventoryUISubMenu);
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(materialPurchasePopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
			});
		}
		else if (!isAvailable)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Destructible material only available for terrain"), string.Empty);
			});
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IMaterialClicked x, BaseEventData y) =>
			{
				x.OnMaterialClicked(id);
			});
		}
	}

	private void PurchaseCallback(bool success, Dictionary<object, object> purchaseData)
	{
		if (success)
		{
			locked = false;
			lockedImage.gameObject.SetActive(value: false);
			OnClick();
		}
	}

	public void OnInfoClick()
	{
		MaterialPurchasePopup materialPurchasePopup = Object.Instantiate(materialPurchasePopupPrefab);
		materialPurchasePopup.Initialize(id, PurchaseCallback);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(materialPurchasePopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
		});
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseHoverDescriptionFrame.SetActive(value: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseHoverDescriptionFrame.SetActive(value: false);
	}
}
