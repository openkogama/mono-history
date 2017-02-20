using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class CenterGunHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.CenterGun, impulse, PlayerKilledByType.CenterGun);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.CenterGun, interactionStruct.Impulse);
	}
}
