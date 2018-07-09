using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.WorldObjectTypes.Avatar.Accessories;
using MV.WorldObject.HighlightSystem;
using MV.WorldObject.HighlightSystem.HighlightPayloads;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccessoryInventoryViewItem : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	private bool locked;

	[SerializeField]
	private RawImage previewImage;

	[SerializeField]
	private StreamPngToSprite previewImageStreaminAssetManual;

	[SerializeField]
	private GameObject loadingWheel;

	[SerializeField]
	private AccessoryItemBackground accessoryItemBackground;

	[SerializeField]
	private Button purchasePopupButton;

	[SerializeField]
	private AvatarAccessoryPurchasePopup purchasePopupPrefab;

	[SerializeField]
	private Toggle equipCheckbox;

	[SerializeField]
	private GameObject priceDisplay;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private GameObject priceStrikeout;

	[SerializeField]
	private Text priceStrikeoutText;

	[SerializeField]
	private AccessoryPreviewer accessoryPreviewerPrefab;

	[SerializeField]
	private GameObject discount;

	[SerializeField]
	private Text discountText;

	[SerializeField]
	private GameObject freeLabel;

	[SerializeField]
	private GameObject newAccessoryImage;

	[SerializeField]
	private GameObject redDotNotification;

	[SerializeField]
	private RawImage levelRequirement;

	[SerializeField]
	private RawImage levelRequirementTooHigh;

	[SerializeField]
	private GameObject priceBackground;

	[SerializeField]
	private AccessoryTimeLimitDisplayer timeLimitDisplayer;

	private Transform rootTransform;

	private AccessoryLoader accessoryLoader = new AccessoryLoader();

	private MVBody targetBody;

	private AccessoryDataClient accessoryDataClient;

	private int highlightId = -1;

	private bool wasDestroyed;

	private float effectDuration = 0.1f;

	public void Initialize(AccessoryDataClient accessoryDataClient, Transform rootTransform, MVBody targetBody, bool bundleView = false)
	{
		this.accessoryDataClient = accessoryDataClient;
		this.rootTransform = rootTransform;
		locked = !accessoryDataClient.owns;
		this.targetBody = targetBody;
		List<Highlight<HighlightAccessory>> highLights = HighlightManager.GetHighLights<HighlightAccessory>(HighlightType.Accessory);
		for (int i = 0; i < highLights.Count; i++)
		{
			if (highLights[i].highlightData.accessoryMetaDataId == accessoryDataClient.accessoryMetaDataID)
			{
				redDotNotification.SetActive(value: true);
				highlightId = highLights[i].id;
			}
		}
		accessoryItemBackground.Initialize(accessoryDataClient);
		newAccessoryImage.SetActive(accessoryDataClient.isNew);
		if (!bundleView)
		{
			timeLimitDisplayer.Initialize(accessoryDataClient.timelimit);
			timeLimitDisplayer.gameObject.SetActive(locked && accessoryDataClient.timelimit.IsTimeLimited);
			bool flag = accessoryDataClient.discount >= 100 || accessoryDataClient.priceGold == 0;
			discount.SetActive(locked && accessoryDataClient.discount > 0 && !flag);
			freeLabel.SetActive(locked && accessoryDataClient.discount > 0 && flag);
			discountText.text = $"-{accessoryDataClient.discount.ToString()}%";
			if (accessoryDataClient.level > 0)
			{
				levelRequirement.gameObject.SetActive(value: true);
				BadgeManager.GetBadgeTexture(accessoryDataClient.level, OnLevelRequirementLoaded);
			}
			equipCheckbox.gameObject.SetActive(!locked);
			if (!locked)
			{
				equipCheckbox.isOn = targetBody.IsAccessoryEquipped(accessoryDataClient.streamingAssetID);
			}
			equipCheckbox.GetComponent<CanvasGroup>().alpha = ((!equipCheckbox.isOn) ? 1f : 0.5f);
			equipCheckbox.onValueChanged.AddListener(OnEquip);
			priceDisplay.SetActive(locked);
			priceStrikeout.SetActive(locked && accessoryDataClient.discount > 0 && accessoryDataClient.priceGold > 0);
			priceStrikeoutText.text = accessoryDataClient.priceGold.ToString();
			priceText.text = accessoryDataClient.DiscountedPrice.ToString();
			if (MVGameControllerBase.Game.LocalPlayer.Level < accessoryDataClient.level)
			{
				priceBackground.SetActive(value: false);
				levelRequirement.gameObject.SetActive(value: false);
				discount.SetActive(value: false);
				levelRequirementTooHigh.gameObject.SetActive(value: true);
			}
		}
		purchasePopupButton.interactable = !bundleView;
		previewImage.gameObject.SetActive(value: false);
		loadingWheel.SetActive(value: true);
		accessoryLoader.LoadAccessory(accessoryDataClient.url, AccessoryCreatedCallback);
	}

	private void OnLevelRequirementLoaded(WWW www)
	{
		if (www == null || www.texture == null)
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement");
			return;
		}
		levelRequirement.texture = www.texture;
		levelRequirementTooHigh.texture = www.texture;
	}

	public void OnClicked()
	{
		if (redDotNotification.activeInHierarchy && highlightId != -1)
		{
			redDotNotification.SetActive(value: false);
			HighlightManager.SetHighlightToSeen(highlightId);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
			{
				x.UpdateHighlightedTab((AccessoryCategoryClient)accessoryDataClient.category);
			});
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
		{
			x.OpenAccessoryManagementScreen(accessoryDataClient);
		});
	}

	public void OnEquip(bool onEquip)
	{
		if (locked)
		{
			return;
		}
		if (onEquip)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAttachToBody x, BaseEventData y) =>
			{
				x.AttachToBody(accessoryDataClient.streamingAssetID, 0f, 1f);
			});
		}
		else
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnAccessoryUnequipped = (Action)Delegate.Combine(game.OnAccessoryUnequipped, new Action(UnequipAccessoryCallback));
			MVGameControllerBase.OperationRequests.UnEquipAccessory(targetBody.Id, accessoryDataClient.accessorySlotType);
		}
	}

	private void UnequipAccessoryCallback()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnAccessoryUnequipped = (Action)Delegate.Remove(game.OnAccessoryUnequipped, new Action(UnequipAccessoryCallback));
		equipCheckbox.isOn = false;
		equipCheckbox.GetComponent<CanvasGroup>().alpha = ((!equipCheckbox.isOn) ? 1f : 0.5f);
	}

	private void OnPurchasePopupPop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IInventoryChanged x, BaseEventData y) =>
		{
			x.InventoryChanged();
		});
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnAccessoryUnequipped = (Action)Delegate.Remove(game.OnAccessoryUnequipped, new Action(UnequipAccessoryCallback));
		}
		wasDestroyed = true;
		accessoryLoader.Destroy();
		accessoryLoader = null;
		BadgeManager.UnsubscribeGetBadgeRequest(OnLevelRequirementLoaded);
	}

	private void AccessoryCreatedCallback(AvatarAccessory avatarAccessory)
	{
		if (wasDestroyed || rootTransform == null)
		{
			UnityEngine.Object.Destroy(avatarAccessory.gameObject);
			return;
		}
		avatarAccessory.transform.parent = rootTransform;
		previewImage.gameObject.SetActive(value: true);
		purchasePopupButton.enabled = true;
		loadingWheel.SetActive(value: false);
		string text = "AccessoryShop/" + accessoryDataClient.category.ToString() + "Images/";
		string[] array = accessoryDataClient.url.Split(new string[1] { "/" }, StringSplitOptions.None);
		array = array[array.Length - 1].Split(new string[1] { "." }, StringSplitOptions.None);
		string text2 = array[0];
		text2 += "Image.png";
		text += text2.ToLower();
		previewImageStreaminAssetManual.Initialize(text);
		previewImageStreaminAssetManual.StartDownloading();
		SkinnedMeshOptimizer[] componentsInChildren = avatarAccessory.GetComponentsInChildren<SkinnedMeshOptimizer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DisableOptimizer();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		StartCoroutine(OnHoverEvent(20f));
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		StartCoroutine(OnHoverEvent(-76f));
	}

	private IEnumerator OnHoverEvent(float sizeOffset)
	{
		float startTime = Time.time;
		Vector2 startSize = previewImage.rectTransform.sizeDelta;
		Vector2 targetSize = new Vector2(sizeOffset, sizeOffset);
		while (Time.time - startTime < effectDuration)
		{
			previewImage.rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, (Time.time - startTime) / effectDuration);
			yield return null;
		}
		previewImage.rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, 1f);
		yield return null;
	}
}
