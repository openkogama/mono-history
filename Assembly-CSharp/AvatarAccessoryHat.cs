using UnityEngine;

public class AvatarAccessoryHat : AvatarAccessory
{
	private Transform attachPosTfm;

	private AccessoryHatSettings hatSettings;

	public override bool HasAttachmentPoint => attachPosTfm != null;

	public AccessoryHatSettings HatSettings
	{
		get
		{
			if (hatSettings == null)
			{
				hatSettings = GetComponent<AccessoryHatSettings>();
			}
			return hatSettings;
		}
	}

	public override AccessorySettings AccessorySettings => HatSettings;

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
		attachPosTfm = Transform.FindChildRecursively("HatAttachPoint");
	}
}
