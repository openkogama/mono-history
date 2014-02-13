using System;
using Localize;
using UnityEngine;

public class MVGUIAvatarAccessoryInventoryGroup : MVGUIAvatarAccessoryBaseGroup
{
	public delegate void OnViewItemActionDelegate(AvatarAccessoryInventoryViewItem viewItem);

	public OnViewItemActionDelegate OnViewItemDragStart;

	public OnViewItemActionDelegate OnViewItemDragEnd;

	public OnViewItemActionDelegate OnViewItemMouseOverEnter;

	public OnViewItemActionDelegate OnViewItemMouseOverExit;

	public TextSlotIndex emptyIndex = TextSlotIndex.Empty;

	public Color textColor;

	private UXText _emptyUIText;

	public AvatarAccessoryInventoryCollection avatarAccessoryInventoryCollection { get; protected set; }

	public override void Initialize()
	{
		base.Initialize();
		InitializeEmptyText();
	}

	protected override void InitializeCollectionView()
	{
		avatarAccessoryInventoryCollection = new AvatarAccessoryInventoryCollection(Game.StreamingAssetInventory);
		collectionView.Initialize();
		collectionView.InstansiateViewItem = InstansiateViewItem;
		UXCollectionView uXCollectionView = collectionView;
		uXCollectionView.OnItemDragStart = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(uXCollectionView.OnItemDragStart, new UXCollectionView.OnBasicItemEventDelegate(OnItemDragStart));
		UXCollectionView uXCollectionView2 = collectionView;
		uXCollectionView2.OnItemDragEnd = (UXCollectionView.OnItemDragEndDelegate)Delegate.Combine(uXCollectionView2.OnItemDragEnd, new UXCollectionView.OnItemDragEndDelegate(OnItemDragEnd));
		UXCollectionView uXCollectionView3 = collectionView;
		uXCollectionView3.OnSlotMouseOverEnter = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(uXCollectionView3.OnSlotMouseOverEnter, new UXCollectionView.OnBasicItemEventDelegate(OnSlotMouseOverEnter));
		UXCollectionView uXCollectionView4 = collectionView;
		uXCollectionView4.OnSlotMouseOverExit = (UXCollectionView.OnBasicItemEventDelegate)Delegate.Combine(uXCollectionView4.OnSlotMouseOverExit, new UXCollectionView.OnBasicItemEventDelegate(OnSlotMouseOverExit));
		collectionView.Collection = avatarAccessoryInventoryCollection;
		collectionView.SetVisible(Group.Visible);
	}

	private void InitializeEmptyText()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/UX/Text"));
		_emptyUIText = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
		((Object)_emptyUIText).name = "EmptyText";
		((Component)_emptyUIText).transform.parent = ((Component)this).transform;
		((Component)_emptyUIText).transform.localScale = Vector3.one;
		((Component)_emptyUIText).transform.localPosition = new Vector3(-3.5f, -1.5f, -0.01f);
		_emptyUIText.Text = Localization.Instance.GetText(emptyIndex);
		_emptyUIText.TextSize = UXTextSize.Medium;
		_emptyUIText.Color = textColor;
	}

	protected override void OnShow()
	{
		base.OnShow();
		if (avatarAccessoryInventoryCollection.CountIncludingEquipped == 0)
		{
			Group.Hide();
		}
		_emptyUIText.SetVisible(avatarAccessoryInventoryCollection.CountIncludingEquipped == 0);
	}

	protected override void OnHide()
	{
		base.OnHide();
		_emptyUIText.SetVisible(avatarAccessoryInventoryCollection.CountIncludingEquipped == 0);
	}

	public override void ResetInventoryGroup()
	{
		avatarAccessoryInventoryCollection = null;
		base.ResetInventoryGroup();
	}

	public override void InitializeAfterReset()
	{
		base.InitializeAfterReset();
		avatarAccessoryInventoryCollection = new AvatarAccessoryInventoryCollection(Game.StreamingAssetInventory);
		collectionView.Collection = avatarAccessoryInventoryCollection;
	}

	private void OnSlotMouseOverEnter(IUXCollectionItem item)
	{
		if (OnViewItemMouseOverEnter != null)
		{
			if (item != null)
			{
				UXCollectionViewItem collectionViewItemFromIndex = collectionView.GetCollectionViewItemFromIndex(item.Index);
				OnViewItemMouseOverEnter((AvatarAccessoryInventoryViewItem)collectionViewItemFromIndex);
			}
			else
			{
				OnViewItemMouseOverEnter(null);
			}
		}
	}

	private void OnSlotMouseOverExit(IUXCollectionItem item)
	{
		if (OnViewItemMouseOverExit != null)
		{
			OnViewItemMouseOverExit(null);
		}
	}

	private void OnItemDragStart(IUXCollectionItem item)
	{
		if (OnViewItemDragStart != null)
		{
			OnViewItemDragStart((AvatarAccessoryInventoryViewItem)collectionView.GetDraggedViewItem());
		}
	}

	private void OnItemDragEnd()
	{
		if (OnViewItemDragEnd != null)
		{
			OnViewItemDragEnd(null);
		}
	}

	protected override UXCollectionViewItem InstansiateViewItem(IUXCollectionItem item)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)inventoryViewItemPrefab);
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.layer = LayerMask.NameToLayer("UXElement");
		val2.transform.parent = ((Component)this).transform;
		val2.transform.localScale = Vector3.one;
		AvatarAccessoryInventoryViewItem component = val2.GetComponent<AvatarAccessoryInventoryViewItem>();
		component.Item = item;
		component.PreviewItemsRoot = previewItemsRoot;
		return component;
	}
}
