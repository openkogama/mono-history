using UnityEngine;

public class MVGUIAvatarShopPreview : MonoBehaviour
{
	public void SetAvatarViewItem(AvatarViewItem avatarViewItem)
	{
		UXPlane uXPlane = Object.Instantiate(avatarViewItem.ItemImagePlane);
		uXPlane.transform.parent = transform;
		uXPlane.transform.localScale = Vector3.one;
		uXPlane.transform.localPosition = Vector3.zero;
	}
}
