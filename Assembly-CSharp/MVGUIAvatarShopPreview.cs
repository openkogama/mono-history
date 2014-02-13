using UnityEngine;

public class MVGUIAvatarShopPreview : MonoBehaviour
{
	public void SetAvatarViewItem(AvatarViewItem avatarViewItem)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		UXPlane uXPlane = Object.Instantiate((Object)(object)avatarViewItem.ItemImagePlane) as UXPlane;
		((Component)uXPlane).transform.parent = ((Component)this).transform;
		((Component)uXPlane).transform.localScale = Vector3.one;
		((Component)uXPlane).transform.localPosition = Vector3.zero;
	}
}
