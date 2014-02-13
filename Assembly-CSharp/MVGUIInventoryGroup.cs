using System;
using System.Linq;
using Localize;
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

	public TextSlotIndex emptyIndex = TextSlotIndex.Empty;

	public string emptyText;

	public Color textColor;

	protected Transform previewItemsRoot;

	private UXText _emptyUIText;

	private MVItem _insertItem;

	public RepositoryCollection repositoryCollection { get; protected set; }

	protected UXGroup Group => ((Component)this).gameObject.GetComponent<UXGroup>();

	public virtual void Initialize()
	{
		if (emptyIndex != TextSlotIndex.Empty)
		{
			emptyText = Localization.Instance.GetText(emptyIndex);
		}
		CreatePreviewItemRoot();
		InitializeCollectionView();
		if (showTextIfEmpty)
		{
			InitializeEmptyText();
		}
	}

	protected void CreatePreviewItemRoot()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		previewItemsRoot = new GameObject("Preview Root - " + ((Object)((Component)this).gameObject).name).transform;
	}

	protected virtual void InitializeCollectionView()
	{
		repositoryCollection = new PlayerRepositoryCollection(MVGameController.Instance.Game.PlayerRepository, allowedCategoriesTypes);
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
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/UX/Text"));
		_emptyUIText = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
		((Object)_emptyUIText).name = "EmptyText";
		((Component)_emptyUIText).transform.parent = ((Component)this).transform;
		((Component)_emptyUIText).transform.localScale = Vector3.one;
		((Component)_emptyUIText).transform.localPosition = new Vector3(0f, ((Component)collectionView).transform.localPosition.y, -0.01f);
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
			((Component)this).gameObject.GetComponent<UXGroup>().Hide();
		}
		_emptyUIText.SetVisible(repositoryCollection.Count == 0);
	}

	private void OnEnable()
	{
		if ((Object)(object)previewItemsRoot != (Object)null)
		{
			((Component)previewItemsRoot).GetComponentsInChildren<Camera>().ToList().ForEach((Camera c) =>
			{
				((Behaviour)c).enabled = true;
			});
		}
	}

	private void OnDisable()
	{
		if ((Object)(object)previewItemsRoot != (Object)null)
		{
			((Component)previewItemsRoot).GetComponentsInChildren<Camera>().ToList().ForEach((Camera c) =>
			{
				((Behaviour)c).enabled = false;
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
		repositoryCollection = new PlayerRepositoryCollection(MVGameController.Instance.Game.PlayerRepository, allowedCategoriesTypes);
		collectionView.Collection = repositoryCollection;
	}

	private void OnSwapItems(IUXCollectionItem sourceItem, IUXCollectionItem destinationItem)
	{
		MVItem mVItem = (MVItem)sourceItem.Object;
		MVItem mVItem2 = (MVItem)destinationItem.Object;
		if (mVItem != null && mVItem2 != null)
		{
			MVGameController.Instance.Game.PlayerRepository.SwapItems(mVItem.itemID, mVItem2.itemID);
		}
		MVGameController.Instance.Game.UpdateInventorySlots();
	}

	private void OnMoveItem(IUXCollectionItem item, int destinationIndex)
	{
		if (item != null)
		{
			MVGameController.Instance.Game.PlayerRepository.MoveItem((item.Object as MVItem).itemID, destinationIndex);
		}
		MVGameController.Instance.Game.UpdateInventorySlots();
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
			MVGameController.Instance.EditController.EditorWorldObjectCreation.OnAddItemFromInventory(_insertItem, isPreviewItem: false);
			if (NotifyItemSelection != null)
			{
				NotifyItemSelection();
			}
		}
	}

	protected virtual UXCollectionViewItem InstansiateViewItem(IUXCollectionItem item)
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
		return component;
	}
}
