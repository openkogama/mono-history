using MV.Common;
using UnityEngine;

public class AvatarAccessoryHat : AvatarAccessory
{
	private Transform attachPosTfm;

	private AccessoryHatSettings hatSettings;

	public override bool HasAttachmentPoint => attachPosTfm != null;

	public override AccessorySettings AccessorySettings => hatSettings;

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
		hatSettings = GetComponent<AccessoryHatSettings>();
		base.Awake();
		Category = AvatarAccessoryCategory.Hat;
		attachPosTfm = Transform.FindChildRecursively("HatAttachPoint");
	}
}
