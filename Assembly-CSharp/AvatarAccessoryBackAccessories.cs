using UnityEngine;

public class AvatarAccessoryBackAccessories : AvatarAccessory
{
	private Transform attachPosTfm;

	private AccessoryBackAccessoriesSettings wingSettings;

	public override bool HasAttachmentPoint => attachPosTfm != null;

	public AccessoryBackAccessoriesSettings WingSettings
	{
		get
		{
			if (wingSettings == null)
			{
				wingSettings = GetComponent<AccessoryBackAccessoriesSettings>();
			}
			return wingSettings;
		}
	}

	public override AccessorySettings AccessorySettings => WingSettings;

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
		attachPosTfm = Transform.FindChildRecursively("WingsAttachPoint");
	}
}
