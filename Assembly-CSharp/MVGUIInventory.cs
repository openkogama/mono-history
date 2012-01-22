using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

[RequireComponent(typeof(UXCollectionView))]
public class MVGUIInventory : UXViewScript
{
	private class PlayerRepositoryCollection : IUXCollection
	{
		private PlayerRepository playerRepository;

		private OnCollectionChangeDelegate onCollectionChange;

		private IUXCollectionItem[] cache = new IUXCollectionItem[0];

		public OnCollectionChangeDelegate OnCollectionChange
		{
			get
			{
				return onCollectionChange;
			}
			set
			{
				onCollectionChange = value;
			}
		}

		public int Count => playerRepository.PlayerInventory.Keys.Count;

		public PlayerRepositoryCollection(PlayerRepository playerRepository)
		{
			this.playerRepository = playerRepository;
			PlayerRepository playerRepository2 = this.playerRepository;
			playerRepository2.OnPlayerRepositoryChange = (PlayerRepository.OnPlayerInventoryChangeDelegate)Delegate.Combine(playerRepository2.OnPlayerRepositoryChange, new PlayerRepository.OnPlayerInventoryChangeDelegate(HandlePlayerRepositoryChange));
			HandlePlayerRepositoryChange(playerRepository);
		}

		public IUXCollectionItem GetItem(int index)
		{
			return cache[index];
		}

		private void HandlePlayerRepositoryChange(PlayerRepository playerRepository)
		{
			cache = new IUXCollectionItem[playerRepository.PlayerInventory.Values.Count];
			int num = 0;
			foreach (KeyValuePair<int, MVItem> item in playerRepository.PlayerInventory)
			{
				cache[num++] = new DefaultCollectionItem((int)playerRepository.itemIDToInventorySlotIndex[item.Key], item.Value);
			}
			NotifyOnCollectionChange();
		}

		private void NotifyOnCollectionChange()
		{
			if (OnCollectionChange != null)
			{
				OnCollectionChange();
			}
		}
	}

	private PlayerRepositoryCollection playerRepositoryCollection;

	private GameObject previewItemsRoot;

	public GameObject inventoryViewItemPrefab;

	public Action OnOpen;

	public Action OnClose;

	private bool firstShow = true;

	public new void Awake()
	{
		UXMouseClickObject component = ((Component)this).GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			Debug.Log((object)"Mouse click");
			return true;
		}));
	}

	public override void OnShow()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected Obj, but got Unknown
		if (firstShow)
		{
			previewItemsRoot = new GameObject("Preview Vault Items Root");
			playerRepositoryCollection = new PlayerRepositoryCollection(MVGameController.Instance.WOCM.PlayerRepository);
			UXCollectionView component = ((Component)this).GetComponent<UXCollectionView>();
			component.InstansiateViewItem = InstansiateViewItem;
			component.OnMoveItem = (UXCollectionView.OnMoveItemDelegate)Delegate.Combine(component.OnMoveItem, new UXCollectionView.OnMoveItemDelegate(OnMoveItem));
			component.OnRemoveItem = (UXCollectionView.OnRemoveItemDelegate)Delegate.Combine(component.OnRemoveItem, new UXCollectionView.OnRemoveItemDelegate(HandleOnRemoveItem));
			component.OnItemSelection = (UXCollectionView.OnItemSelectionDelegate)Delegate.Combine(component.OnItemSelection, new UXCollectionView.OnItemSelectionDelegate(OnItemSelection));
			component.Collection = playerRepositoryCollection;
			firstShow = false;
		}
	}

	private void HandleOnRemoveItem(IUXCollectionItem item)
	{
		int itemID = (item.Object as MVItem).itemID;
		MVGameController.Instance.Game.RemoveItemFromInventory(itemID);
	}

	private void OnMoveItem(int sourceSlotIndex, int destinationSlotIndex)
	{
	}

	private void OnItemSelection(IUXCollectionItem item)
	{
		View.Hide();
		if (OnClose != null)
		{
			OnClose();
		}
		MVGameController.Instance.EditorController.OnAddPrototypeFromInventory(item.Object as MVItem);
		Debug.Log((object)((Object)((Component)this).gameObject).name);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private UXCollectionViewItem InstansiateViewItem(IUXCollectionItem item)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)inventoryViewItemPrefab);
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.layer = LayerMask.NameToLayer("UXElement");
		val2.transform.parent = ((Component)this).transform;
		val2.transform.localScale = Vector3.one;
		InventoryViewItem component = val2.GetComponent<InventoryViewItem>();
		component.Item = item;
		component.PreviewItemsRoot = previewItemsRoot;
		component.Initialize();
		return component;
	}
}
