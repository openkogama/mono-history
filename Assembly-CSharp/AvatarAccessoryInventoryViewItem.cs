using UnityEngine;

public class AvatarAccessoryInventoryViewItem : AvatarAccessoryBasicViewItem
{
	private MVGUIAvatarAccessoryRentTimer avatarAccessoryRentTimer;

	protected MVNetworkGame Game => MVGameController.Game;

	public int InventoryID { get; private set; }

	public ProductInventoryInfo InventoryInfo => Game.StreamingAssetInventory.Get(InventoryID);

	public override void Initialize()
	{
		if (!_isBuilding)
		{
			base.Initialize();
			if (InventoryInfo.IsRented)
			{
				CreateExpirationTimer(Game.StreamingAssetExpirationChecker.GetExpirationInfo(InventoryInfo.InventoryID));
			}
		}
	}

	protected override void GetStreamingAssetInfo()
	{
		InventoryID = (int)Item.Object;
		streamingAssetInfo = InventoryInfo.ProductInfo;
	}

	protected override void BuildViewItem()
	{
		AddTooltip(streamingAssetInfo.Name);
		_loading = true;
		LoadingCircle.SetVisible(Visible);
		AvatarAccessory.Create(InventoryInfo, OnAvatarAccessoryCreated);
	}

	private void CreateExpirationTimer(InventoryExpirationInfo inventoryExpirationInfo)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarAccessory/AvatarAccessoryRentTimer"));
		gameObject.transform.parent = transform;
		gameObject.transform.localPosition = new Vector3(0f, (0f - Height) / 2f, 0f);
		gameObject.transform.localScale = Vector3.one;
		avatarAccessoryRentTimer = gameObject.GetComponent<MVGUIAvatarAccessoryRentTimer>();
		avatarAccessoryRentTimer.InitializeRentTimer(inventoryExpirationInfo);
		avatarAccessoryRentTimer.SetVisible(Visible);
	}

	public override void OnAttachToSlot(UXCollectionViewSlot slot)
	{
		base.OnAttachToSlot(slot);
		if (avatarAccessoryRentTimer != null)
		{
			avatarAccessoryRentTimer.SetVisible(Visible);
		}
	}

	public override void OnDetachFromSlot(UXCollectionViewSlot slot)
	{
		base.OnDetachFromSlot(slot);
		if (avatarAccessoryRentTimer != null)
		{
			avatarAccessoryRentTimer.SetVisible(visible: false);
		}
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if (avatarAccessoryRentTimer != null)
		{
			avatarAccessoryRentTimer.SetVisible(visible);
		}
	}
}
