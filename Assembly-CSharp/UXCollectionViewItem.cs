using UnityEngine;

public abstract class UXCollectionViewItem : MonoBehaviour
{
	public OnCollectionViewItemDelegate OnSelection;

	public OnCollectionViewItemDelegate OnRemove;

	public OnCollectionViewItemDelegate OnMouseOver;

	public abstract IUXCollectionItem Item { get; set; }
}
