using MV.WorldObject;
using UnityEngine;

public class SentryTowerIcePackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.SentryTowerIce, impulse);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, interactionStruct.Impulse, AvatarModifierPackageType.Frozen);
	}
}
