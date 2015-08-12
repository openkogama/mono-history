using System;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVGUIInventoryGroup : MonoBehaviour
{
	public delegate void OnItemSelectionDelegate();

	public delegate void CanInsertDelegate(bool canInsert);

	public OnItemSelectionDelegate NotifyItemSelection;

	public UXCollectionView collectionView;

	public GameObject inventoryViewItemPrefab;

	public int[] allowedCategoriesTypes;

	public bool showTextIfEmpty;

	public string emptyText;

	public Color textColor;

	protected Transform previewItemsRoot;

	private UXText _emptyUIText;

	private MVItem _insertItem;

	public RepositoryCollection repositoryCollection { get; protected set; }

	protected UXGroup Group => gameObject.GetComponent<UXGroup>();

	public virtual void Initialize()
	{
		emptyText = TM._(emptyText);
		CreatePreviewItemRoot();
		InitializeCollectionView();
		if (showTextIfEmpty)
		{
			InitializeEmptyText();
		}
	}

	protected void CreatePreviewItemRoot()
	{
		previewItemsRoot = new GameObject("Preview Root - " + gameObject.name).transform;
	}

	protected virtual void InitializeCollectionView()
	{
		repositoryCollection = new PlayerRepositoryCollection(MVGameController.Game.PlayerRepository, allowedCategoriesTypes);
		collectionView.Initialize();
		collectionView.InstansiateViewItem = InstansiateViewItem;
		UXCollectionView uXCollectionView = collectionView;
		uXCollectionView.OnMoveItem = (UXCollectionView.OnMoveItemDelegate)Delegate.Combine(uXCollectionView.OnMoveItem, new UXCollectionView.OnMoveItemDelegate(OnMoveItem));
		UXCollectionView uXCollectionView2 = collectionView;
		uXCollectionView2.OnSwapItems = (UXCollectionView.OnSwapItemsDelegate)Delegate.Combine(uXCollectionView2.OnSwapItems, new UXCollectionView.OnSwapItemsDelegate(OnSwapItems));
		UXCollectionView uXCollectionView3 = collectionView;
		uXCollectionView3.OnItemSelection = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(uXCollectionView3.OnItemSelection, new UXCollectionView.OnBasicItemEventDelegate(OnItemSelection));
		collectionView.Collection = repositoryCollection;
		collectionView.SetVisible(Group.Visible);
	}

	private void InitializeEmptyText()
	{
		_emptyUIText = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/UX/Text")) as GameObject).GetComponent<UXText>();
		_emptyUIText.name = "EmptyText";
		_emptyUIText.transform.parent = transform;
		_emptyUIText.transform.localScale = Vector3.one;
		_emptyUIText.transform.localPosition = new Vector3(0f, collectionView.transform.localPosition.y, -0.01f);
		_emptyUIText.Text = emptyText;
		_emptyUIText.TextSize = UXTextSize.Large;
		_emptyUIText.Color = textColor;
		UXGroup uXGroup = Group;
		uXGroup.OnShowGroup = (UXGroup.OnGroupEventDelegate)Delegate.Combine(uXGroup.OnShowGroup, new UXGroup.OnGroupEventDelegate(OnShowInventoryGroup));
	}

	private void OnShowInventoryGroup()
	{
		if (repositoryCollection.Count == 0)
		{
			gameObject.GetComponent<UXGroup>().Hide();
		}
		_emptyUIText.SetVisible(repositoryCollection.Count == 0);
	}

	private void OnEnable()
	{
		if (previewItemsRoot != null)
		{
			previewItemsRoot.GetComponentsInChildren<Camera>().ToList().ForEach((Camera c) =>
			{
				c.enabled = true;
			});
		}
	}

	private void OnDisable()
	{
		if (previewItemsRoot != null)
		{
			previewItemsRoot.GetComponentsInChildren<Camera>().ToList().ForEach((Camera c) =>
			{
				c.enabled = false;
			});
		}
	}

	public void ResetInventoryGroup()
	{
		previewItemsRoot = null;
		repositoryCollection = null;
		collectionView.ResetViewItems();
	}

	public virtual void InitializeAfterReset()
	{
		CreatePreviewItemRoot();
		repositoryCollection = new PlayerRepositoryCollection(MVGameController.Game.PlayerRepository, allowedCategoriesTypes);
		collectionView.Collection = repositoryCollection;
	}

	private void OnSwapItems(IUXCollectionItem sourceItem, IUXCollectionItem destinationItem)
	{
		MVItem mVItem = (MVItem)sourceItem.Object;
		MVItem mVItem2 = (MVItem)destinationItem.Object;
		if (mVItem != null && mVItem2 != null)
		{
			MVGameController.Game.PlayerRepository.SwapItems(mVItem.itemID, mVItem2.itemID);
		}
		MVGameController.Game.UpdateInventorySlots();
	}

	private void OnMoveItem(IUXCollectionItem item, int destinationIndex)
	{
		if (item != null)
		{
			MVGameController.Game.PlayerRepository.MoveItem((item.Object as MVItem).itemID, destinationIndex);
		}
		MVGameController.Game.UpdateInventorySlots();
	}

	private void OnItemSelection(IUXCollectionItem item)
	{
		InventoryViewItem inventoryViewItem = (InventoryViewItem)collectionView.GetCollectionViewItemFromIndex(item.Index);
		_insertItem = (MVItem)item.Object;
		inventoryViewItem.WO.CheckCanInsert(OnCanInsertItem);
	}

	private void OnCanInsertItem(bool canInsert)
	{
		if (canInsert)
		{
			MVGameController.EditorController.EditorWorldObjectCreation.OnAddItemFromInventory(_insertItem, isPreviewItem: false);
			if (NotifyItemSelection != null)
			{
				NotifyItemSelection();
			}
		}
	}

	protected virtual UXCollectionViewItem InstansiateViewItem(IUXCollectionItem item)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(inventoryViewItemPrefab);
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.transform.localScale = Vector3.one;
		InventoryViewItem component = gameObject.GetComponent<InventoryViewItem>();
		component.Item = item;
		component.PreviewItemsRoot = previewItemsRoot;
		return component;
	}
}
