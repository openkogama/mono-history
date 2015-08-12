using UnityEngine;

public class MVGUIShopInventoryPreview : MonoBehaviour
{
	private InventoryViewItem inventoryViewItem;

	public void SetInventoryViewItem(InventoryViewItem inventoryViewItem)
	{
		this.inventoryViewItem = inventoryViewItem;
		UXPlane uXPlane = Object.Instantiate(inventoryViewItem.ItemImagePlane);
		uXPlane.transform.parent = transform;
		uXPlane.transform.localScale = Vector3.one;
		uXPlane.transform.localPosition = Vector3.zero;
	}

	private void Update()
	{
		if (inventoryViewItem != null)
		{
			inventoryViewItem.ObjectPreviewer.UpdateRotation();
		}
	}
}
