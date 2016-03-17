using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarPurchasePopup : MonoBehaviour
{
	[SerializeField]
	private Text avatarGoldCost;

	[SerializeField]
	private RawImage avatarImage;

	private AvatarRepositoryItem item;

	public void Initialize(RawImage image, AvatarRepositoryItem item)
	{
		this.item = item;
		avatarGoldCost.text = item.priceGold.ToString();
		avatarImage.texture = image.texture;
	}

	public void OnPurchaseClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPurchaseAvatar x, BaseEventData y) =>
		{
			x.PurchaseAvatar(item);
		});
	}
}
