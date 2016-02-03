using MV.Common;
using UnityEngine;

public class AvatarAccessoryHat : AvatarAccessory
{
	private Transform attachPosTfm;

	private AccessoryHatSettings hatSettings;

	private static readonly Shader accessoryShader = Shader.Find("Diffuse with vertex colors");

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
		Category = AvatarAccessoryCategory.Hat;
		attachPosTfm = Transform.FindChildRecursively("HatAttachPoint");
		Renderer[] renderers = Renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.material.shader = accessoryShader;
		}
	}
}
