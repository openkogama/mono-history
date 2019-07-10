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
	private Image previewImage;

	[SerializeField]
	private StreamedSpriteToImageManual previewImageStreamingManager;

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

	[SerializeField]
	private AvatarAccessoryErrorPopup touristErrorPopup;

	[SerializeField]
	private AccessoryShinyButton buttonAnimation;

	[SerializeField]
	private GameObject lockIcon;

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
			HandlePreviewing(MVGameControllerBase.LocalPlayer.Body);
		}
		goldSavedText.gameObject.SetActive(value: false);
		nameText.text = accessoryData.name.ToUpper();
		offsetSlider.Initialize(accessoryData.slot, accessoryData.sAID);
		sizeSlider.Initialize(accessoryData.slot, accessoryData.sAID);
		accessoryItemBackground.Initialize(accessoryData);
		SetShowNotOwnedUI(shouldShow: false);
		purchaseButton.gameObject.SetActive(!accessoryData.owns);
		levelRequirementPurchaseButton.gameObject.SetActive(value: false);
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
		if (accessoryDataClient != null && accessoryDataClient.aMDID == data.aMDID)
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
		if (accessoryDataClient != null && accessoryDataClient.owns && !avatarBody.IsAccessoryEquipped(accessoryDataClient.sAID))
		{
			AvatarAccessoryEquipPopup popup = UnityEngine.Object.Instantiate(avatarAccessoryEquipPopup);
			float accessoryOffset = avatarBody.GetAccessoryOffset(accessoryDataClient.slot);
			float accessoryScale = avatarBody.GetAccessoryScale(accessoryDataClient.slot);
			popup.Initialize(EquipPopupResultCallback, previewImageUrl, accessoryDataClient, accessoryOffset, accessoryScale);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
			});
		}
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
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
		}
	}

	private void HandlePreviewing(MVBody avatarBody)
	{
		this.avatarBody = avatarBody;
		isPreviewing = !avatarBody.IsAccessoryEquipped(accessoryDataClient.sAID);
		sizeSlider.IsInPreview = isPreviewing;
		offsetSlider.IsInPreview = isPreviewing;
		if (!avatarBody.IsAccessoryEquipped(accessoryDataClient.sAID))
		{
			avatarBody.PreviewAccessory(accessoryDataClient);
		}
	}

	private void OnLevelRequirementLoaded(WWW www)
	{
		Texture2D texture = www.texture;
		if (texture == null)
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement");
		}
		else
		{
			levelRequirementPurchaseButton.texture = texture;
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
		if (MVGameControllerBase.Game.LocalPlayer.IsTourist)
		{
			AvatarAccessoryErrorPopup errorPopup = UnityEngine.Object.Instantiate(touristErrorPopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(errorPopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			errorPopup.Initialize(OnTouristSignupClicked, previewImageUrl, accessoryDataClient, TM._("Signup required"), TM._("Sign up"));
		}
		else if (MVGameControllerBase.Game.LocalPlayer.Level >= accessoryDataClient.lvl)
		{
			Purchase();
		}
		else
		{
			LevelErrorPopup errorPopup2 = UnityEngine.Object.Instantiate(insufficientLevelPopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(errorPopup2.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			errorPopup2.Initialize(null, accessoryDataClient.lvl);
		}
	}

	public void Purchase()
	{
		if (MVGameControllerBase.Game.LocalPlayer.UserProfileData.Gold >= accessoryDataClient.DiscountedPrice)
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
			BrowserCommGotoRequests.GotoPurchaseGold(newTab: false, modalPopup: true);
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
				if (!value[i].owns && value[i].iFtr)
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
		string text = "AvatarAccessory/" + accessoryDataClient.cat.ToString() + "/Images/";
		string[] array = accessoryDataClient.url.Split(new string[1] { "/" }, StringSplitOptions.None);
		array = array[array.Length - 1].Split(new string[1] { "." }, StringSplitOptions.None);
		string text2 = array[0];
		text2 += "Image.unity3d";
		text += text2.ToLower();
		SkinnedMeshOptimizer[] componentsInChildren = avatarAccessory.GetComponentsInChildren<SkinnedMeshOptimizer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DisableOptimizer();
		}
		previewImageStreamingManager.Download(text, OnPreviewImageFinishedDownloading);
		previewImageUrl = text;
	}

	private void OnPreviewImageFinishedDownloading()
	{
		loadingWheel.SetActive(value: false);
		emptyFrame.SetActive(value: false);
		ShowLoadedStreamingAssetsObject();
	}

	private void HandlePrices(AccessoryDataClient streamingAssetInfo)
	{
		buttonAnimation.gameObject.SetActive(value: true);
		int cost = streamingAssetInfo.cost;
		int dsc = streamingAssetInfo.dsc;
		int num = cost;
		originalPriceText.gameObject.SetActive(dsc > 0);
		goldSavedText.gameObject.SetActive(dsc > 0);
		discountTag.SetActive(dsc > 0);
		claimText.gameObject.SetActive(value: false);
		purchaseButton.image.color = Styles.GetColor(ColorStyle.ButtonSuccess);
		lockIcon.SetActive(value: false);
		if (dsc > 0)
		{
			discountTagText.text = ((dsc < 100) ? ("-" + dsc + "%") : "FREE");
			int num2 = Mathf.FloorToInt((float)cost * ((float)dsc / 100f));
			num = cost - num2;
			originalPriceText.text = cost.ToString("N0").Replace(",", " ");
			goldSavedText.gameObject.SetActive(value: true);
			goldSavedText.text = num2.ToString("N0").Replace(",", " ");
			priceTextWithoutDiscount.gameObject.SetActive(value: false);
			priceText.gameObject.SetActive(value: true);
		}
		else
		{
			priceTextWithoutDiscount.gameObject.SetActive(value: true);
			priceText.gameObject.SetActive(value: false);
		}
		priceText.text = num.ToString("N0").Replace(",", " ");
		priceTextWithoutDiscount.text = num.ToString("N0").Replace(",", " ");
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
		buttonAnimation.gameObject.SetActive(shouldShow);
		priceTextWithoutDiscount.gameObject.SetActive(shouldShow);
		priceText.gameObject.SetActive(shouldShow);
		goldSavedText.gameObject.SetActive(shouldShow);
		originalPriceText.gameObject.SetActive(shouldShow);
		discountTag.SetActive(shouldShow);
		claimText.gameObject.SetActive(shouldShow);
		levelRequirementPurchaseButton.gameObject.SetActive(!shouldShow);
		claimText.gameObject.SetActive(shouldShow);
		purchaseButton.image.color = Styles.GetColor(ColorStyle.ButtonSuccess);
		if (!shouldShow)
		{
			lockIcon.SetActive(!shouldShow);
			purchaseButton.image.color = Styles.GetColor(ColorStyle.DisabledButton);
			BadgeManager.GetBadgeTexture(accessoryDataClient.lvl, OnLevelRequirementLoaded);
		}
	}

	private void HandleNotOwnedUI()
	{
		if (MVGameControllerBase.Game.LocalPlayer.Level >= accessoryDataClient.lvl)
		{
			HandlePrices(accessoryDataClient);
		}
		else
		{
			SetShowPrices(shouldShow: false);
		}
		timeLimitDisplayer.Initialize(accessoryDataClient.time);
		timeLimitDisplayer.gameObject.SetActive(!accessoryDataClient.owns && accessoryDataClient.time.IsTimeLimited);
		newAccessoryImage.SetActive(accessoryDataClient.iNew);
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
			timeLimitDisplayer.gameObject.SetActive(accessoryDataClient.time.IsTimeLimited);
			newAccessoryImage.SetActive(accessoryDataClient.iNew);
			discountTag.SetActive(accessoryDataClient.dsc > 0 && MVGameControllerBase.Game.LocalPlayer.Level >= accessoryDataClient.lvl);
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

	private void OnTouristSignupClicked(bool confirmed)
	{
		if (confirmed)
		{
			BrowserCommGotoRequests.GotoSignup(newTab: false, modalPopup: true);
		}
	}
}
