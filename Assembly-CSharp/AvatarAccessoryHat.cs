using MV.Common;
using UnityEngine;

[AddComponentMenu("KoGaMa/AvatarAccessories/Hat")]
public class AvatarAccessoryHat : AvatarAccessory
{
	private Transform attachPosTfm;

	public override bool HasAttachmentPoint => attachPosTfm != null;

	public override Vector3 AttachmentPointWorldPos
	{
		get
		{
			if (attachPosTfm != null)
			{
				return attachPosTfm.position;
			}
			return Vector3.zero;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Category = AvatarAccessoryCategory.Hat;
		attachPosTfm = Transform.FindChildRecursively("HatAttachPoint");
	}
}
