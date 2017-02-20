using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ShotgunHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.ShotgunHit, impulse, PlayerKilledByType.Shotgun);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.Shotgun, interactionStruct.Impulse);
	}
}
