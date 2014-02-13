using System;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessorySlot : UXPlane
{
	private const float MOUSE_OVER_ALPHA = 0.1f;

	private const float STANDARD_ALPHA = 0.8f;

	private const float DOWNPLAY_ALPHA = 0.1f;

	private int _inventoryIDOfItemInSlot = -1;

	public AvatarAccessorySlot avatarAccessorySlot;

	public UXIconButton unequipAvatarAccessoryButton;

	public UXPlane occupiedIndicator;

	public Color standardColor = Color.black;

	public Color rentedItemColor;

	private bool _itemDroppedInSlot;

	private AvatarAccessoryInventoryViewItem _dragItem;

	private bool validSlot;

	private MVGUIAvatarAccessoryRentTimer rentTimer;

	private bool waitForEquip;

	private ProductInventoryInfo<StreamingAssetInfo> waitingToBeEquipped;

	private MVNetworkGame Game => MVGameController.Instance.Game;

	private CharacterEditorController CEController => MVGameController.Instance.CharacterEditorController;

	private MVBody AvatarBody
	{
		get
		{
			if ((Object)(object)MVGameController.Instance == (Object)null || CEController == null)
			{
				return null;
			}
			return CEController.CurrentBody;
		}
	}

	private bool SlotOccupied => InventoryIDOfItemInSlot != -1;

	private bool RentedItemInSlot { get; set; }

	private int InventoryIDOfItemInSlot
	{
		get
		{
			return _inventoryIDOfItemInSlot;
		}
		set
		{
			_inventoryIDOfItemInSlot = value;
			RemoveExpirationTimer();
			if (_inventoryIDOfItemInSlot != -1)
			{
				ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo = Game.StreamingAssetInventory.Get(InventoryIDOfItemInSlot);
				if (productInventoryInfo != null)
				{
					RentedItemInSlot = productInventoryInfo.IsRented;
				}
				if (RentedItemInSlot)
				{
					CreateExpirationTimer(InventoryIDOfItemInSlot);
				}
			}
		}
	}

	public MVGUIAvatarAccessorySlot()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		UXIconButton uXIconButton = unequipAvatarAccessoryButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, new UXBaseButton.OnClickDelegate(Unequip));
		UXDropObject component = ((Component)this).GetComponent<UXDropObject>();
		component.AcceptDrop = (UXDropObject.AcceptDropDelegate)Delegate.Combine(component.AcceptDrop, new UXDropObject.AcceptDropDelegate(AcceptDrop));
		component.OnDrop = (UXDropObject.OnDropDelegate)Delegate.Combine(component.OnDrop, new UXDropObject.OnDropDelegate(OnDrop));
		UXMouseOverObject component2 = ((Component)this).GetComponent<UXMouseOverObject>();
		component2.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component2.OnMouseOverEnter, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject obj) =>
		{
			OnMouseOverEnter();
		}));
		component2.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component2.OnMouseOverExit, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject obj) =>
		{
			OnMouseOverExit();
		}));
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		unequipAvatarAccessoryButton.SetVisible(visible && SlotOccupied);
		occupiedIndicator.SetVisible(visible && SlotOccupied);
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		base.SetAlpha(alpha, materialProperty);
		occupiedIndicator.SetAlpha(alpha, "_MainColor");
		unequipAvatarAccessoryButton.SetAlpha(alpha);
	}

	public void HightlightSlot(bool validSlot)
	{
		this.validSlot = validSlot;
		if (!validSlot)
		{
			SetAlpha(0.1f);
		}
		else if (SlotOccupied)
		{
			occupiedIndicator.SetVisible(visible: false);
			unequipAvatarAccessoryButton.SetVisible(visible: false);
		}
	}

	public void StopSlotHighlight()
	{
		SetAlpha(0.8f);
		SetVisible(Visible);
	}

	public void SetStopDragItem()
	{
		if (_itemDroppedInSlot)
		{
			Equip(_dragItem.InventoryInfo);
			_itemDroppedInSlot = false;
		}
	}

	private void OnMouseOverEnter()
	{
		SetAlpha(0.90000004f);
		if ((Object)(object)rentTimer != (Object)null)
		{
			rentTimer.SetVisible(Visible);
		}
	}

	private void OnMouseOverExit()
	{
		SetStopDragItem();
		if ((Object)(object)rentTimer != (Object)null)
		{
			rentTimer.SetVisible(visible: false);
		}
	}

	private void CreateExpirationTimer(int inventoryID)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected Obj, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		InventoryExpirationInfo expirationInfo = Game.StreamingAssetExpirationChecker.GetExpirationInfo(inventoryID);
		GameObject val = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarAccessory/AvatarAccessoryRentTimer"));
		val.transform.parent = ((Component)this).transform;
		val.transform.localPosition = new Vector3(0f, (0f - Height) / 2f, 0f);
		val.transform.localScale = Vector3.one;
		rentTimer = val.GetComponent<MVGUIAvatarAccessoryRentTimer>();
		rentTimer.InitializeRentTimer(expirationInfo);
		rentTimer.SetVisible(visible: false);
	}

	private void RemoveExpirationTimer()
	{
		if (!((Object)(object)rentTimer == (Object)null))
		{
			Object.Destroy((Object)(object)((Component)rentTimer).gameObject);
			rentTimer = null;
		}
	}

	public void RefreshEquippedState()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (AvatarBody != null && AvatarBody.AnyAccessoryInSlot(avatarAccessorySlot))
		{
			InventoryIDOfItemInSlot = AvatarBody.GetAccessoryID(avatarAccessorySlot);
		}
		else
		{
			InventoryIDOfItemInSlot = -1;
		}
		if (SlotOccupied)
		{
			occupiedIndicator.SetColor((!RentedItemInSlot) ? standardColor : rentedItemColor, string.Empty);
		}
		unequipAvatarAccessoryButton.SetVisible(Visible && SlotOccupied);
		occupiedIndicator.SetVisible(Visible && SlotOccupied);
		SetAlpha(0.8f);
	}

	private void Equip(ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo)
	{
		if (!waitForEquip)
		{
			if (SlotOccupied)
			{
				waitingToBeEquipped = productInventoryInfo;
				waitForEquip = true;
				Unequip();
			}
			else
			{
				AvatarAccessory.Create(productInventoryInfo, OnAvatarAccessoryCreated);
			}
		}
	}

	private void OnAvatarAccessoryCreated(AvatarAccessory avatarAccessory)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		Game.SetAvatarAccessorySlot(AvatarBody.Id, avatarAccessory.InventoryID, avatarAccessorySlot, avatarAccessory.DefaultOffset);
		AvatarBody.AttachAccessory(avatarAccessory, avatarAccessorySlot, avatarAccessory.DefaultOffset);
		unequipAvatarAccessoryButton.SetVisible(Visible);
		occupiedIndicator.SetVisible(Visible);
		InventoryIDOfItemInSlot = avatarAccessory.InventoryID;
		if (SlotOccupied)
		{
			occupiedIndicator.SetColor((!RentedItemInSlot) ? standardColor : rentedItemColor, string.Empty);
		}
	}

	private void Unequip()
	{
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(OnUnequipItemResponse));
		Game.SetAvatarAccessorySlot(AvatarBody.Id, InventoryIDOfItemInSlot, AvatarAccessorySlot.Undefined, 0f);
	}

	private void OnUnequipItemResponse(bool setSlotSuccess)
	{
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(OnUnequipItemResponse));
		if (setSlotSuccess)
		{
			unequipAvatarAccessoryButton.SetVisible(visible: false);
			occupiedIndicator.SetVisible(visible: false);
			InventoryIDOfItemInSlot = -1;
			if (waitForEquip)
			{
				waitForEquip = false;
				Equip(waitingToBeEquipped);
				waitingToBeEquipped = null;
			}
		}
		else
		{
			waitForEquip = false;
			waitingToBeEquipped = null;
		}
	}

	private void Game_OnSetAvatarAccessorySlotResponseEquipHandler(bool setSlotSuccess)
	{
		MVNetworkGame game = Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		if (!setSlotSuccess)
		{
			AvatarBody.DestroyAccessory(InventoryIDOfItemInSlot);
			unequipAvatarAccessoryButton.SetVisible(visible: false);
			occupiedIndicator.SetVisible(visible: false);
			InventoryIDOfItemInSlot = -1;
		}
	}

	public void SetDragItem(AvatarAccessoryInventoryViewItem dragItem)
	{
		_dragItem = dragItem;
	}

	private bool AcceptDrop(GameObject dropObject)
	{
		if ((Object)(object)_dragItem != (Object)null)
		{
			return (Object)(object)_dragItem.AvatarAccessory != (Object)null && validSlot;
		}
		return false;
	}

	private void OnDrop(GameObject dropObject)
	{
		if ((Object)(object)_dragItem != (Object)null)
		{
			_itemDroppedInSlot = true;
		}
	}
}
