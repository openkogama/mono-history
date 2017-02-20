using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class SlapGunHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.SlapGunHit, impulse, PlayerKilledByType.SlapGun);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.SlapGun, interactionStruct.Impulse);
	}
}
