using System;
using System.Collections.Generic;
using Assets.Scripts.WorldObjectTypes.Avatar.Accessories;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccessoryView : MonoBehaviour
{
	private AccessoryDataClient accessoryDataClient;

	[SerializeField]
	private RawImage previewImage;

	[SerializeField]
	private StreamPngToSprite previewImageStreamingManager;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Button purchaseButton;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Text priceTextWithoutDiscount;

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
	private RawImage levelRequirementPurchaseButton;

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

	[SerializeField]
	private AvatarAccessoryEquipPopup avatarAccessoryEquipPopup;

	[SerializeField]
	private AvatarAccessoryErrorPopup insufficientResourcePopup;

	[SerializeField]
	private LevelErrorPopup insufficientLevelPopup;

	[SerializeField]
	private GameObject shopCloseButton;

	[SerializeField]
	private GameObject loadingWheel;

	[SerializeField]
	private GameObject emptyFrame;

	[SerializeField]
	private Text claimText;

	private AccessoryPreviewer previewer;

	private Transform rootTransform;

	private AccessoryLoader accessoryLoader = new AccessoryLoader();

	private Action OnFinished;

	private bool isPreviewing;

	private MVBody avatarBody;

	private string previewImageUrl;

	public void Initialize(AccessoryDataClient accessoryData)
	{
		if (rootTransform != null)
		{
			UnityEngine.Object.Destroy(rootTransform.gameObject);
		}
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
		nameText.text = accessoryData.name.ToUpper();
		offsetSlider.Initialize(accessoryData.accessorySlotType, accessoryData.streamingAssetID);
		sizeSlider.Initialize(accessoryData.accessorySlotType, accessoryData.streamingAssetID);
		accessoryItemBackground.Initialize(accessoryData);
		SetShowNotOwnedUI(shouldShow: false);
		purchaseButton.gameObject.SetActive(!accessoryData.owns);
		if (!accessoryDataClient.owns)
		{
			SetShowNotOwnedUI(shouldShow: true);
			HandleNotOwnedUI();
		}
		else
		{
			SetShowNotOwnedUI(shouldShow: false);
		}
		HideNotLoadedStreamingAssetsObject();
		loadingWheel.SetActive(value: true);
		emptyFrame.SetActive(value: true);
		accessoryLoader.LoadAccessory(accessoryData.url, AvatarAccessoryCreateHandler);
	}

	public bool CurrentlyViewingAccessory(AccessoryDataClient data)
	{
		if (accessoryDataClient != null && accessoryDataClient.accessoryMetaDataID == data.accessoryMetaDataID)
		{
			return true;
		}
		return false;
	}

	private void OnEnable()
	{
		TabMenuButtonBase tabMenuButton = tabMenu.GetTabMenuButton(AccessoryCategoryClient.Bundles);
		if (tabMenuButton != null)
		{
			tabMenuButton.gameObject.SetActive(value: false);
		}
		shopCloseButton.SetActive(value: false);
	}

	private void OnDisable()
	{
		shopCloseButton.SetActive(value: true);
		if (accessoryDataClient != null && accessoryDataClient.owns && !avatarBody.IsAccessoryEquipped(accessoryDataClient.streamingAssetID))
		{
			AvatarAccessoryEquipPopup popup = UnityEngine.Object.Instantiate(avatarAccessoryEquipPopup);
			float accessoryOffset = avatarBody.GetAccessoryOffset(accessoryDataClient.accessorySlotType);
			float accessoryScale = avatarBody.GetAccessoryScale(accessoryDataClient.accessorySlotType);
			popup.Initialize(EquipPopupResultCallback, previewImageUrl, accessoryDataClient, accessoryOffset, accessoryScale);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
			});
		}
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
			if (rootTransform != null)
			{
				UnityEngine.Object.Destroy(rootTransform.gameObject);
				rootTransform = null;
			}
			if (previewer != null)
			{
				UnityEngine.Object.Destroy(previewer.gameObject);
			}
			if (accessoryLoader != null)
			{
				accessoryLoader.Destroy();
			}
			accessoryDataClient = null;
			previewImageStreamingManager.DestroyTexture();
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
		}
		else
		{
			levelRequirementPurchaseButton.texture = www.texture;
		}
	}

	public void Destroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(OnLevelRequirementLoaded);
		if (rootTransform != null)
		{
			UnityEngine.Object.Destroy(rootTransform.gameObject);
		}
		rootTransform = null;
		if (previewer != null)
		{
			UnityEngine.Object.Destroy(previewer.gameObject);
		}
		if (accessoryLoader != null)
		{
			accessoryLoader.Destroy();
		}
	}

	public void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	public void OnPurchaseButtonPressed()
	{
		if (MVGameControllerBase.Game.LocalPlayer.Level >= accessoryDataClient.level)
		{
			Purchase();
			return;
		}
		LevelErrorPopup errorPopup = UnityEngine.Object.Instantiate(insufficientLevelPopup);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(errorPopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
		errorPopup.Initialize(OnInsufficientLevelCallback, accessoryDataClient.level);
	}

	public void Purchase()
	{
		if (MVGameControllerBase.Game.LocalPlayer.GoldAmount >= accessoryDataClient.DiscountedPrice)
		{
			AvatarAccessoryPurchasePopup popUp = UnityEngine.Object.Instantiate(AvatarAccessoryPurchasePopupPrefab);
			popUp.Initialize(accessoryDataClient, previewImageUrl);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popUp.gameObject, UIPushOption.Blocking, BackToShop, UIGroupFlags.InventoryUISubMenu);
			});
		}
		else
		{
			AvatarAccessoryErrorPopup popUp2 = UnityEngine.Object.Instantiate(insufficientResourcePopup);
			popUp2.Initialize(OnGoldPurchaseDialogResult, previewImageUrl, accessoryDataClient, TM._("Not enough gold"), TM._("Get gold"));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popUp2.gameObject, UIPushOption.Blocking, BackToShop, UIGroupFlags.InventoryUISubMenu);
			});
		}
	}

	private void OnGoldPurchaseDialogResult(bool result)
	{
		if (result)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoPurchaseGold");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.purchaseGoldURL);
		}
	}

	public void BackToShop()
	{
		if (rootTransform != null)
		{
			UnityEngine.Object.Destroy(rootTransform.gameObject);
		}
		rootTransform = null;
		if (accessoryLoader != null)
		{
			accessoryLoader.Destroy();
		}
		if (previewer != null)
		{
			previewer.Destroy();
		}
		if (tabMenu.GetTabMenuButton(AccessoryCategoryClient.Featured) != null)
		{
			DestroyFeaturedTab();
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
		{
			x.OpenCategoryScreen(canSortByInventory: true);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryInventoryControl x, BaseEventData y) =>
		{
			x.RefreshItems();
		});
	}

	private void DestroyFeaturedTab()
	{
		Dictionary<AccessoryCategory, List<AccessoryDataClient>> accessoriesCategoryMap = AccessoryDataManager.GetAccessoriesCategoryMap();
		bool flag = false;
		foreach (List<AccessoryDataClient> value in accessoriesCategoryMap.Values)
		{
			for (int i = 0; i < value.Count; i++)
			{
				if (!value[i].owns && value[i].isFeatured)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			tabMenu.DestroyTab(AccessoryCategoryClient.Featured);
		}
	}

	private void EquipPopupResultCallback()
	{
		if (accessoryDataClient == null)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryClicked x, BaseEventData y) =>
			{
				x.OpenCategoryScreen(canSortByInventory: true);
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryInventoryControl x, BaseEventData y) =>
			{
				x.RefreshItems();
			});
		}
	}

	private void AvatarAccessoryCreateHandler(AvatarAccessory avatarAccessory)
	{
		if (rootTransform == null)
		{
			UnityEngine.Object.Destroy(avatarAccessory.gameObject);
			return;
		}
		previewer = UnityEngine.Object.Instantiate(accessoryPreviewerPrefab);
		previewer.transform.SetParent(rootTransform);
		previewer.Initialize(512, 512, LayerFlags.Default | LayerFlags.CamRotateTarget, CameraClearFlags.Color, new Vector3(0f, 2.8f, -3.5f), new Vector3(-3f, 0f, 0f), avatarAccessory.gameObject, rootTransform);
		if (OnFinished != null)
		{
			OnFinished();
		}
		OnFinished = null;
		string text = "AccessoryShop/" + accessoryDataClient.category.ToString() + "Images/";
		string[] array = accessoryDataClient.url.Split(new string[1] { "/" }, StringSplitOptions.None);
		array = array[array.Length - 1].Split(new string[1] { "." }, StringSplitOptions.None);
		string text2 = array[0];
		text2 += "Image.png";
		text += text2.ToLower();
		StreamPngToSprite streamPngToSprite = previewImageStreamingManager;
		streamPngToSprite.OnDownloadFinish = (Action)Delegate.Combine(streamPngToSprite.OnDownloadFinish, new Action(OnPreviewImageFinishedDownloading));
		previewImageStreamingManager.StartDownloading(text);
		previewImageUrl = text;
		SkinnedMeshOptimizer[] componentsInChildren = avatarAccessory.GetComponentsInChildren<SkinnedMeshOptimizer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DisableOptimizer();
		}
	}

	private void OnPreviewImageFinishedDownloading()
	{
		loadingWheel.SetActive(value: false);
		emptyFrame.SetActive(value: false);
		ShowLoadedStreamingAssetsObject();
	}

	private void HandlePrices(AccessoryDataClient streamingAssetInfo)
	{
		int priceGold = streamingAssetInfo.priceGold;
		int discount = streamingAssetInfo.discount;
		int num = priceGold;
		originalPriceText.gameObject.SetActive(discount > 0);
		goldSavedText.gameObject.SetActive(discount > 0);
		discountTag.SetActive(discount > 0);
		claimText.gameObject.SetActive(value: false);
		if (discount > 0)
		{
			discountTagText.text = ((discount < 100) ? ("-" + discount + "%") : "FREE");
			int num2 = Mathf.FloorToInt((float)priceGold * ((float)discount / 100f));
			num = priceGold - num2;
			originalPriceText.text = priceGold.ToString("N0");
			goldSavedText.gameObject.SetActive(value: true);
			goldSavedText.text = num2.ToString("N0");
			priceTextWithoutDiscount.gameObject.SetActive(value: false);
			priceText.gameObject.SetActive(value: true);
		}
		else
		{
			priceTextWithoutDiscount.gameObject.SetActive(value: true);
			priceText.gameObject.SetActive(value: false);
		}
		priceText.text = num.ToString("N0");
		priceTextWithoutDiscount.text = num.ToString("N0");
		levelRequirementPurchaseButton.gameObject.SetActive(value: false);
		if (num == 0)
		{
			goldSavedText.gameObject.SetActive(value: false);
			priceTextWithoutDiscount.gameObject.SetActive(value: false);
			priceText.gameObject.SetActive(value: false);
			discountTag.SetActive(value: false);
			originalPriceText.gameObject.SetActive(value: false);
			claimText.gameObject.SetActive(value: true);
		}
	}

	private void SetShowPrices(bool shouldShow)
	{
		priceTextWithoutDiscount.gameObject.SetActive(shouldShow);
		priceText.gameObject.SetActive(shouldShow);
		goldSavedText.gameObject.SetActive(shouldShow);
		originalPriceText.gameObject.SetActive(shouldShow);
		discountTag.SetActive(shouldShow);
		claimText.gameObject.SetActive(shouldShow);
		levelRequirementPurchaseButton.gameObject.SetActive(!shouldShow);
		claimText.gameObject.SetActive(shouldShow);
		if (!shouldShow)
		{
			BadgeManager.GetBadgeTexture(accessoryDataClient.level, OnLevelRequirementLoaded);
		}
	}

	private void HandleNotOwnedUI()
	{
		if (MVGameControllerBase.Game.LocalPlayer.Level >= accessoryDataClient.level)
		{
			HandlePrices(accessoryDataClient);
		}
		else
		{
			SetShowPrices(shouldShow: false);
		}
		timeLimitDisplayer.Initialize(accessoryDataClient.timelimit);
		timeLimitDisplayer.gameObject.SetActive(!accessoryDataClient.owns && accessoryDataClient.timelimit.IsTimeLimited);
		newAccessoryImage.SetActive(accessoryDataClient.isNew);
	}

	private void HideNotLoadedStreamingAssetsObject()
	{
		timeLimitDisplayer.gameObject.SetActive(value: false);
		newAccessoryImage.SetActive(value: false);
		discountTag.SetActive(value: false);
	}

	private void ShowLoadedStreamingAssetsObject()
	{
		if (!accessoryDataClient.owns)
		{
			timeLimitDisplayer.gameObject.SetActive(accessoryDataClient.timelimit.IsTimeLimited);
			newAccessoryImage.SetActive(accessoryDataClient.isNew);
			discountTag.SetActive(accessoryDataClient.discount > 0 && MVGameControllerBase.Game.LocalPlayer.Level >= accessoryDataClient.level);
		}
	}

	private void SetShowNotOwnedUI(bool shouldShow)
	{
		purchaseButton.gameObject.SetActive(shouldShow);
		timeLimitDisplayer.gameObject.SetActive(shouldShow);
		newAccessoryImage.SetActive(shouldShow);
		originalPriceText.gameObject.SetActive(shouldShow);
		discountTag.SetActive(shouldShow);
		goldSavedText.gameObject.SetActive(shouldShow);
	}

	private void OnInsufficientLevelCallback()
	{
	}
}
