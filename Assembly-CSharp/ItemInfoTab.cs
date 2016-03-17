using UnityEngine;
using UnityEngine.UI;

public class ItemInfoTab : ManageItemPage
{
	[SerializeField]
	private Text itemName;

	[SerializeField]
	private Text description;

	[SerializeField]
	private RawImage previewImage;

	public override void Initialize(RawImage preview, InventoryItem item)
	{
		previewImage.texture = preview.texture;
		itemName.text = item.name;
		description.text = item.description;
	}
}
