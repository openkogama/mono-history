using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.WorldObjectTypes.Avatar.Accessories;
using MV.WorldObject.HighlightSystem;
using MV.WorldObject.HighlightSystem.HighlightPayloads;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccessoryInventoryViewItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
	private bool locked;

	[SerializeField]
	private RectTransform previewImage;

	[SerializeField]
	private StreamedSpriteToImageManual previewImageStreaminAssetManual;

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
	private AccessoryTimeLimitDisplayer timeLimitDisplayer;

	private Transform rootTransform;

	private AccessoryLoader accessoryLoader = new AccessoryLoader();

	private MVBody targetBody;

	private AccessoryDataClient accessoryDataClient;

	private Texture2D levelRequirementTextureAsset;

	private int highlightId = -1;

	private bool bundleView;

	private bool wasDestroyed;

	private float effectDuration = 0.1f;

	public void Initialize(AccessoryDataClient accessoryDataClient, Transform rootTransform, MVBody targetBody, bool bundleView = false)
	{
		this.accessoryDataClient = accessoryDataClient;
		this.rootTransform = rootTransform;
		locked = !accessoryDataClient.owns;
		this.targetBody = targetBody;
		this.bundleView = bundleView;
		List<Highlight<HighlightAccessory>> highLights = HighlightManager.GetHighLights<HighlightAccessory>(HighlightType.Accessory);
		for (int i = 0; i < highLights.Count; i++)
		{
			if (highLights[i].highlightData.accessoryMetaDataId == accessoryDataClient.aMDID)
			{
				redDotNotification.SetActive(locked);
				highlightId = highLights[i].id;
			}
		}
		purchasePopupButton.interactable = !bundleView;
		previewImage.gameObject.SetActive(value: false);
		accessoryItemBackground.gameObject.SetActive(value: false);
		loadingWheel.SetActive(value: true);
		accessoryLoader.LoadAccessory(accessoryDataClient.url, AccessoryCreatedCallback);
	}

	private void SetLevelBadge()
	{
		LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Remove(LevelingManager.OnLevelingInitialized, new UnityAction(SetLevelBadge));
		BadgeManager.GetBadgeTexture(accessoryDataClient.lvl, OnLevelRequirementLoaded);
	}

	private void OnLevelRequirementLoaded(UnityWebRequest www)
	{
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement, Error: " + www.error);
		}
		else if (levelRequirement != null && !wasDestroyed)
		{
			levelRequirementTextureAsset = DownloadHandlerTexture.GetContent(www);
			levelRequirement.gameObject.SetActive(value: true);
			levelRequirement.texture = levelRequirementTextureAsset;
		}
	}

	public void OnClicked()
	{
		if (redDotNotification.activeInHierarchy && highlightId != -1)
		{
			redDotNotification.SetActive(value: false);
			HighlightManager.SetHighlightToSeen(highlightId);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
			{
				x.UpdateHighlightedTab((AccessoryCategoryClient)accessoryDataClient.cat);
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
				x.AttachToBody(accessoryDataClient.sAID, 0f, 1f);
			});
		}
		else
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnAccessoryUnequipped = (Action)Delegate.Combine(game.OnAccessoryUnequipped, new Action(UnequipAccessoryCallback));
			MVGameControllerBase.OperationRequests.UnEquipAccessory(targetBody.Id, accessoryDataClient.slot);
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
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnAccessoryUnequipped = (Action)Delegate.Remove(game.OnAccessoryUnequipped, new Action(UnequipAccessoryCallback));
		}
		wasDestroyed = true;
		accessoryLoader.Destroy();
		accessoryLoader = null;
		BadgeManager.UnsubscribeGetBadgeRequest(OnLevelRequirementLoaded);
		levelRequirementTextureAsset = null;
	}

	private void AccessoryCreatedCallback(AvatarAccessory avatarAccessory)
	{
		if (wasDestroyed || rootTransform == null)
		{
			UnityEngine.Object.Destroy(avatarAccessory.gameObject);
			return;
		}
		avatarAccessory.transform.parent = rootTransform;
		string text = "AvatarAccessory/" + accessoryDataClient.cat.ToString() + "/Images/";
		string[] array = accessoryDataClient.url.Split(new string[1] { "/" }, StringSplitOptions.None);
		array = array[array.Length - 1].Split(new string[1] { "." }, StringSplitOptions.None);
		string text2 = array[0];
		text2 += "Image.unity3d";
		text += text2.ToLower();
		previewImageStreaminAssetManual.Download(text, OnPreviewImageDownloadFinished);
		SkinnedMeshOptimizer[] componentsInChildren = avatarAccessory.GetComponentsInChildren<SkinnedMeshOptimizer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DisableOptimizer();
		}
	}

	private void OnPreviewImageDownloadFinished()
	{
		if (wasDestroyed)
		{
			return;
		}
		previewImage.gameObject.SetActive(value: true);
		loadingWheel.SetActive(value: false);
		accessoryItemBackground.gameObject.SetActive(value: true);
		accessoryItemBackground.Initialize(accessoryDataClient);
		newAccessoryImage.SetActive((accessoryDataClient.iNew || accessoryDataClient.time.IsTimeLimited) && !accessoryDataClient.owns);
		if (bundleView)
		{
			return;
		}
		if (accessoryDataClient.lvl != 0 && locked)
		{
			if (LevelingManager.IsInitialized)
			{
				SetLevelBadge();
			}
			else
			{
				LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, new UnityAction(SetLevelBadge));
			}
		}
		Rect rect = ((RectTransform)transform).rect;
		Vector3 localScale = new Vector3(rect.width / 400f, rect.height / 400f, 1f);
		discount.transform.localScale = localScale;
		timeLimitDisplayer.transform.localScale = localScale;
		if (MVGameControllerBase.Game.LocalPlayer.Level >= accessoryDataClient.lvl)
		{
			timeLimitDisplayer.Initialize(accessoryDataClient.time);
			timeLimitDisplayer.gameObject.SetActive(locked && accessoryDataClient.time.IsTimeLimited);
			bool flag = accessoryDataClient.dsc >= 100 || accessoryDataClient.cost == 0;
			discount.SetActive(locked && accessoryDataClient.dsc > 0 && !flag);
			freeLabel.SetActive(locked && flag);
			discountText.text = $"-{accessoryDataClient.dsc.ToString()}%";
			equipCheckbox.gameObject.SetActive(!locked);
			if (!locked)
			{
				equipCheckbox.isOn = targetBody.IsAccessoryEquipped(accessoryDataClient.sAID);
			}
			equipCheckbox.GetComponent<CanvasGroup>().alpha = ((!equipCheckbox.isOn) ? 1f : 0.5f);
			equipCheckbox.onValueChanged.AddListener(OnEquip);
			priceDisplay.SetActive(locked);
			priceStrikeout.SetActive(locked && accessoryDataClient.dsc > 0 && accessoryDataClient.cost > 0);
			priceStrikeoutText.text = accessoryDataClient.cost.ToString("N0").Replace(",", " ");
			priceText.text = accessoryDataClient.DiscountedPrice.ToString("N0").Replace(",", " ");
			levelRequirement.rectTransform.localPosition = new Vector2(levelRequirement.rectTransform.localPosition.x, levelRequirement.rectTransform.localPosition.y + ((RectTransform)priceDisplay.transform).rect.height);
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

	public void OnPointerClick(PointerEventData eventData)
	{
	}

	private IEnumerator OnHoverEvent(float sizeOffset)
	{
		float startTime = Time.time;
		Vector2 startSize = previewImage.sizeDelta;
		Vector2 targetSize = new Vector2(sizeOffset, sizeOffset);
		while (Time.time - startTime < effectDuration)
		{
			previewImage.sizeDelta = Vector2.Lerp(startSize, targetSize, (Time.time - startTime) / effectDuration);
			yield return null;
		}
		previewImage.sizeDelta = Vector2.Lerp(startSize, targetSize, 1f);
		yield return null;
	}
}
