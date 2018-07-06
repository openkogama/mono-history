using MV.Common;
using UnityEngine;

public class SelectionHelperAvatarAccessory : MonoBehaviour
{
	private AvatarAccessory avatarAccessory;

	private AccessorySlotType slot;

	private int avatarBodyWoID;

	[SerializeField]
	[HideInInspector]
	private int streamingAssetsId = -1;

	public AvatarAccessory AvatarAccessory => avatarAccessory;

	public AccessorySlotType Slot => slot;

	public int AvatarBodyWoID => avatarBodyWoID;

	public int StreamingAssetsId => streamingAssetsId;

	public void Init(AvatarAccessory avatarAccessory, AccessorySlotType slot, int avatarBodyWoID, int accessoryStreamingAssetsId)
	{
		Debug.Log("Selection helper initialized! " + gameObject.name);
		this.avatarAccessory = avatarAccessory;
		this.slot = slot;
		this.avatarBodyWoID = avatarBodyWoID;
		streamingAssetsId = accessoryStreamingAssetsId;
	}

	public override string ToString()
	{
		return string.Format("AvatarBodyWoID " + avatarBodyWoID);
	}
}
