using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPreview : MonoBehaviour
{
	[SerializeField]
	protected Text title;

	[SerializeField]
	protected Text description;

	[SerializeField]
	protected RawImage previewImage;

	protected InventoryItem item;

	public virtual void Initialize(InventoryItem inventoryItem, RawImage preview)
	{
		item = inventoryItem;
		title.text = item.name;
		description.text = item.description;
		previewImage.texture = preview.mainTexture;
	}
}
