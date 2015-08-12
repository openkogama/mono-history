using MV.WorldObject;

public class AvatarViewItem : InventoryViewItem
{
	private const float AVATAR_ROTATE_SPEED = 20f;

	public UXText nameText;

	public override void Initialize()
	{
		if (!_isBuilding)
		{
			_isBuilding = true;
			BuildImagePlane();
			MVItem item = Item.Object as MVItem;
			BuildViewItem(item, 128, 256);
		}
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		nameText.SetVisible(visible);
	}

	protected override void OnInventoryViewItemBuilt()
	{
		base.OnInventoryViewItemBuilt();
		MVItem mVItem = Item.Object as MVItem;
		UXText componentInChildren = GetComponentInChildren<UXText>();
		componentInChildren.Text = mVItem.name;
	}

	protected override void AddListeners()
	{
	}

	public override void OnAttachToSlot(UXCollectionViewSlot slot)
	{
	}

	public override void OnDetachFromSlot(UXCollectionViewSlot slot)
	{
	}

	public override void Update()
	{
		base.Update();
		if (ObjectPreviewer != null)
		{
			ObjectPreviewer.UpdateRotation(20f);
		}
	}
}
