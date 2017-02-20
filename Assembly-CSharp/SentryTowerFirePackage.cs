using MV.WorldObject;
using UnityEngine;

public class SentryTowerFirePackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.SentryTowerFire, impulse);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Impulse, AvatarModifierPackageType.Fire);
	}
}
