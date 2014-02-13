using MV.Common;
using UnityEngine;

[AddComponentMenu("KoGaMa/AvatarAccessories/Hat")]
public class AvatarAccessoryHat : AvatarAccessory
{
	private Transform attachPosTfm;

	public override bool HasAttachmentPoint => (Object)(object)attachPosTfm != (Object)null;

	public override Vector3 AttachmentPointWorldPos
	{
		get
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)attachPosTfm != (Object)null)
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
