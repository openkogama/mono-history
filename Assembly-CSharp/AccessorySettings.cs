using MV.Common;
using UnityEngine;

public abstract class AccessorySettings : MonoBehaviour
{
	public AvatarAccessorySlot[] ValidSlots;

	public bool AllignToBody;

	public float DefaultOffset;

	public AvatarAccessorySlot DefaultSlot = AvatarAccessorySlot.Torso;

	public bool ConstantWorldRotation;
}
