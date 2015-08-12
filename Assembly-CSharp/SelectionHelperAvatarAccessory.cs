using MV.Common;
using UnityEngine;

public class SelectionHelperAvatarAccessory : MonoBehaviour
{
	private AvatarAccessory avatarAccessory;

	private AvatarAccessorySlot slot;

	private int avatarBodyWoID;

	public AvatarAccessory AvatarAccessory => avatarAccessory;

	public AvatarAccessorySlot Slot => slot;

	public int AvatarBodyWoID => avatarBodyWoID;

	public void Init(AvatarAccessory avatarAccessory, AvatarAccessorySlot slot, int avatarBodyWoID)
	{
		this.avatarAccessory = avatarAccessory;
		this.slot = slot;
		this.avatarBodyWoID = avatarBodyWoID;
	}

	public override string ToString()
	{
		return string.Format("AvatarBodyWoID " + avatarBodyWoID);
	}
}
