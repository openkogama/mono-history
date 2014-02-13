using UnityEngine;

public class MVGUIShopInventoryPreview : MonoBehaviour
{
	private InventoryViewItem inventoryViewItem;

	public void SetInventoryViewItem(InventoryViewItem inventoryViewItem)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		this.inventoryViewItem = inventoryViewItem;
		UXPlane uXPlane = Object.Instantiate((Object)(object)inventoryViewItem.ItemImagePlane) as UXPlane;
		((Component)uXPlane).transform.parent = ((Component)this).transform;
		((Component)uXPlane).transform.localScale = Vector3.one;
		((Component)uXPlane).transform.localPosition = Vector3.zero;
	}

	private void Update()
	{
		if ((Object)(object)inventoryViewItem != (Object)null)
		{
			inventoryViewItem.ObjectPreviewer.UpdateRotation();
		}
	}
}
