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

	private ProductInventoryInfo waitingToBeEquipped;

	private MVNetworkGame Game => MVGameController.Game;

	private CharacterEditorController CEController => MVGameController.CharacterEditorController;

	private MVBody AvatarBody
	{
		get
		{
			if (MVGameController.GameMode == MVGameMode.CharacterEditor)
			{
				return CEController.CurrentBody;
			}
			return MVGameController.WOCM.AvatarLocal.Body;
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
				ProductInventoryInfo productInventoryInfo = Game.StreamingAssetInventory.Get(InventoryIDOfItemInSlot);
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

	private void Start()
	{
		UXIconButton uXIconButton = unequipAvatarAccessoryButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, new UXBaseButton.OnClickDelegate(Unequip));
		UXDropObject component = GetComponent<UXDropObject>();
		component.AcceptDrop = (UXDropObject.AcceptDropDelegate)Delegate.Combine(component.AcceptDrop, new UXDropObject.AcceptDropDelegate(AcceptDrop));
		component.OnDrop = (UXDropObject.OnDropDelegate)Delegate.Combine(component.OnDrop, new UXDropObject.OnDropDelegate(OnDrop));
		UXMouseOverObject component2 = GetComponent<UXMouseOverObject>();
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
		occupiedIndicator.SetAlpha(alpha, string.Empty);
		unequipAvatarAccessoryButton.SetAlpha(alpha, string.Empty);
	}

	public void HightlightSlot(bool validSlot)
	{
		this.validSlot = validSlot;
		if (!validSlot)
		{
			SetAlpha(0.1f, string.Empty);
		}
		else if (SlotOccupied)
		{
			occupiedIndicator.SetVisible(visible: false);
			unequipAvatarAccessoryButton.SetVisible(visible: false);
		}
	}

	public void StopSlotHighlight()
	{
		SetAlpha(0.8f, string.Empty);
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
		SetAlpha(0.90000004f, string.Empty);
		if (rentTimer != null)
		{
			rentTimer.SetVisible(Visible);
		}
	}

	private void OnMouseOverExit()
	{
		SetStopDragItem();
		if (rentTimer != null)
		{
			rentTimer.SetVisible(visible: false);
		}
	}

	private void CreateExpirationTimer(int inventoryID)
	{
		InventoryExpirationInfo expirationInfo = Game.StreamingAssetExpirationChecker.GetExpirationInfo(inventoryID);
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarAccessory/AvatarAccessoryRentTimer"));
		gameObject.transform.parent = transform;
		gameObject.transform.localPosition = new Vector3(0f, (0f - Height) / 2f, 0f);
		gameObject.transform.localScale = Vector3.one;
		rentTimer = gameObject.GetComponent<MVGUIAvatarAccessoryRentTimer>();
		rentTimer.InitializeRentTimer(expirationInfo);
		rentTimer.SetVisible(visible: false);
	}

	private void RemoveExpirationTimer()
	{
		if (!(rentTimer == null))
		{
			UnityEngine.Object.Destroy(rentTimer.gameObject);
			rentTimer = null;
		}
	}

	public void RefreshEquippedState()
	{
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
		SetAlpha(0.8f, string.Empty);
	}

	private void Equip(ProductInventoryInfo productInventoryInfo)
	{
		if (Application.isEditor && productInventoryInfo.ProductInfo.IsEditorPreview)
		{
			if (SlotOccupied)
			{
				AvatarBody.EditorSwapAccessoryAssetPath(InventoryIDOfItemInSlot, productInventoryInfo.ProductInfo.AssetPath);
			}
		}
		else if (!waitForEquip)
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
		if (_dragItem != null)
		{
			return _dragItem.AvatarAccessory != null && validSlot;
		}
		return false;
	}

	private void OnDrop(GameObject dropObject)
	{
		if (_dragItem != null)
		{
			_itemDroppedInSlot = true;
		}
	}
}
