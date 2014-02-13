using UnityEngine;

public class AvatarAccessoryInventoryViewItem : AvatarAccessoryBasicViewItem
{
	private MVGUIAvatarAccessoryRentTimer avatarAccessoryRentTimer;

	protected MVNetworkGame Game => MVGameController.Instance.Game;

	public int InventoryID { get; private set; }

	public ProductInventoryInfo<StreamingAssetInfo> InventoryInfo => Game.StreamingAssetInventory.Get(InventoryID);

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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected Obj, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarAccessory/AvatarAccessoryRentTimer"));
		val.transform.parent = ((Component)this).transform;
		val.transform.localPosition = new Vector3(0f, (0f - Height) / 2f, 0f);
		val.transform.localScale = Vector3.one;
		avatarAccessoryRentTimer = val.GetComponent<MVGUIAvatarAccessoryRentTimer>();
		avatarAccessoryRentTimer.InitializeRentTimer(inventoryExpirationInfo);
		avatarAccessoryRentTimer.SetVisible(Visible);
	}

	public override void OnAttachToSlot(UXCollectionViewSlot slot)
	{
		base.OnAttachToSlot(slot);
		if ((Object)(object)avatarAccessoryRentTimer != (Object)null)
		{
			avatarAccessoryRentTimer.SetVisible(Visible);
		}
	}

	public override void OnDetachFromSlot(UXCollectionViewSlot slot)
	{
		base.OnDetachFromSlot(slot);
		if ((Object)(object)avatarAccessoryRentTimer != (Object)null)
		{
			avatarAccessoryRentTimer.SetVisible(visible: false);
		}
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if ((Object)(object)avatarAccessoryRentTimer != (Object)null)
		{
			avatarAccessoryRentTimer.SetVisible(visible);
		}
	}
}
