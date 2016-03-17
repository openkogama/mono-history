using UnityEngine;
using UnityEngine.UI;

public abstract class ManageItemPage : MonoBehaviour
{
	public abstract void Initialize(RawImage image, InventoryItem item);
}
