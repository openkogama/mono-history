using System;
using Assets.Scripts.WorldObjectTypes.Avatar.Accessories;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccessoryView : MonoBehaviour
{
	private AccessoryDataClient accessoryDataClient;

	[SerializeField]
	private Image previewImage;

	[SerializeField]
	private StreamedSpriteToImageManual previewImageStreamingManager;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private GameObject purchaseButton;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Text originalPriceText;

	[SerializeField]
	private GameObject discountTag;

	[SerializeField]
	private Text discountTagText;

	[SerializeField]
	private Text goldSavedText;

	[SerializeField]
	private AccessoryOffsetSlider offsetSlider;

	[SerializeField]
	private AccessorySizeSlider sizeSlider;

	[SerializeField]
	private AccessoryItemBackground accessoryItemBackground;

	[SerializeField]
	private AccessoryTimeLimitDisplayer timeLimitDisplayer;

	[SerializeField]
	private RawImage levelRequirement;

	[SerializeField]
	private GameObject newAccessoryImage;

	[SerializeField]
	private TabMenuAccessoryShop tabMenu;

	[SerializeField]
	private AvatarAccessoryPurchasePopup AvatarAccessoryPurchasePopupPrefab;

	[SerializeField]
	private AccessoryPreviewer accessoryPreviewerPrefab;

	[SerializeField]
	private PlayerCurrentGoldAmountTracker currentGoldAmountTracker;

	private AccessoryPreviewer previewer;

	private Transform rootTransform;

	private AccessoryLoader accessoryLoader = new AccessoryLoader();

	private Action OnFinished;

	private bool isPreviewing;

	private MVBody avatarBody;

	public void Initialize(AccessoryDataClient accessoryData)
	{
		rootTransform = new GameObject().transform;
		rootTransform.gameObject.name = "Accessory_Preview";
		accessoryDataClient = accessoryData;
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(HandlePreviewing);
			});
		}
		else
		{
			HandlePreviewing(MVGameControllerBase.WOCM.AvatarLocal.Body);
		}
		goldSavedText.gameObject.SetActive(value: false);
		purchaseButton.SetActive(!accessoryData.owns);
		nameText.text = accessoryData.name.ToUpper();
		offsetSlider.Initialize(accessoryData.accessorySlotType, accessoryData.streamingAssetID);
		sizeSlider.Initialize(accessoryData.accessorySlotType, accessoryData.streamingAssetID);
		accessoryItemBackground.Initialize(accessoryData);
		if (!accessoryData.owns)
		{
			HandleNotOwnedUI();
		}
		accessoryLoader.LoadAccessory(accessoryData.url, AvatarAccessoryCreateHandler);
	}

	private void OnEnable()
	{
		TabMenuButtonBase tabMenuButton = tabMenu.GetTabMenuButton(AccessoryCategoryClient.Bundles);
		if (tabMenuButton != null)
		{
			tabMenuButton.gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		if (MVGameControllerBase.Game != null)
		{
			if (isPreviewing)
			{
				avatarBody.EndPreviewAccessory();
			}
			TabMenuButtonBase tabMenuButton = tabMenu.GetTabMenuButton(AccessoryCategoryClient.Bundles);
			if (tabMenuButton != null)
			{
				tabMenuButton.gameObject.SetActive(value: true);
			}
		}
	}

	private void HandlePreviewing(MVBody avatarBody)
	{
		this.avatarBody = avatarBody;
		isPreviewing = !avatarBody.IsAccessoryEquipped(accessoryDataClient.streamingAssetID);
		sizeSlider.IsInPreview = isPreviewing;
		offsetSlider.IsInPreview = isPreviewing;
		if (!avatarBody.IsAccessoryEquipped(accessoryDataClient.streamingAssetID))
		{
			avatarBody.PreviewAccessory(accessoryDataClient);
		}
	}

	private void OnLevelRequirementLoaded(WWW www)
	{
		if (www == null || www.texture == null)
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement");
			return;
		}
		levelRequirement.texture = www.texture;
		levelRequirement.gameObject.SetActive(value: true);
	}

	public void AttachAccessory(AccessoryDataClient purchasedItem, Action OnFinishedCallback)
	{
	}

	public void Destroy()
	{
		if (rootTransform != null)
		{
			UnityEngine.Object.Destroy(rootTransform.gameObject);
		}
		rootTransform = null;
		if (previewer != null)
		{
			UnityEngine.Object.Destroy(previewer.gameObject);
		}
		accessoryLoader.Destroy();
	}

	public void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	public void Purchase()
	{
		AvatarAccessoryPurchasePopup popUp = UnityEngine.Object.Instantiate(AvatarAccessoryPurchasePopupPrefab);
		popUp.Initialize(accessoryDataClient, previewImage.sprite, RefreshGoldAmount);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popUp.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
		});
	}

	private void RefreshGoldAmount()
	{
		currentGoldAmountTracker.RefreshGoldAmount();
	}

	public void BackToShop()
	{
		if (rootTransform != null)
		{
			UnityEngine.Object.Destroy(rootTransform.gameObject);
		}
		rootTransform = null;
		previewer.Destroy();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
		{
			x.OpenCategoryScreen(canSortByInventory: true);
		});
	}

	private void AvatarAccessoryCreateHandler(AvatarAccessory avatarAccessory)
	{
		if (rootTransform == null)
		{
			UnityEngine.Object.Destroy(avatarAccessory.gameObject);
			return;
		}
		previewer = UnityEngine.Object.Instantiate(accessoryPreviewerPrefab);
		previewer.Initialize(512, 512, LayerFlags.Default | LayerFlags.CamRotateTarget, CameraClearFlags.Color, new Vector3(0f, 2.8f, -3.5f), new Vector3(-3f, 0f, 0f), avatarAccessory.gameObject, rootTransform);
		if (OnFinished != null)
		{
			OnFinished();
		}
		OnFinished = null;
		previewImageStreamingManager.Initialize(avatarAccessory.PreviewImageStreamPath);
		previewImageStreamingManager.StartDownloading();
	}

	private void HandlePrices(AccessoryDataClient streamingAssetInfo)
	{
		int priceGold = streamingAssetInfo.priceGold;
		int discount = streamingAssetInfo.discount;
		int num = priceGold;
		originalPriceText.gameObject.SetActive(discount > 0);
		discountTag.SetActive(discount > 0);
		if (discount > 0)
		{
			discountTagText.text = ((discount < 100) ? ("-" + discount + "%") : "FREE");
			int num2 = Mathf.FloorToInt((float)priceGold * ((float)discount / 100f));
			num = priceGold - num2;
			originalPriceText.text = priceGold.ToString("N0");
			goldSavedText.gameObject.SetActive(value: true);
			goldSavedText.text = num2.ToString("N0");
		}
		priceText.text = num.ToString("N0");
	}

	private void HandleNotOwnedUI()
	{
		HandlePrices(accessoryDataClient);
		timeLimitDisplayer.Initialize(accessoryDataClient.timelimit);
		timeLimitDisplayer.gameObject.SetActive(accessoryDataClient.timelimit.IsTimeLimited);
		if (accessoryDataClient.level > 0)
		{
			BadgeManager.GetBadgeTexture(accessoryDataClient.level, OnLevelRequirementLoaded);
		}
		newAccessoryImage.SetActive(accessoryDataClient.isNew);
	}

	private void HideNotOwnedUI()
	{
		purchaseButton.SetActive(value: false);
		timeLimitDisplayer.gameObject.SetActive(value: false);
		levelRequirement.gameObject.SetActive(value: false);
		newAccessoryImage.SetActive(value: false);
		originalPriceText.gameObject.SetActive(value: false);
		discountTag.SetActive(value: false);
		goldSavedText.gameObject.SetActive(value: false);
	}
}
