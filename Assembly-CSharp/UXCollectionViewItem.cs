using UnityEngine;

public abstract class UXCollectionViewItem : MonoBehaviour
{
	public float Width;

	public float Height;

	public bool Visible;

	public bool Deleted { get; set; }

	public abstract IUXCollectionItem Item { get; set; }

	public bool IsInitialized { get; set; }

	public int PageIndex { get; set; }

	public abstract void OnAttachToSlot(UXCollectionViewSlot slot);

	public abstract void OnDetachFromSlot(UXCollectionViewSlot slot);

	public virtual void OnMouseSlotOverEnter(int slotIndex, int visiblePage)
	{
	}

	public virtual void OnMouseSlotOver(int slotIndex, int visiblePage)
	{
	}

	public virtual void OnMouseSlotOverExit(int slotIndex, int visiblePage)
	{
	}

	public abstract void Initialize();

	public virtual void SetVisible(bool visible)
	{
		Visible = visible;
	}
}
