using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class SixShooterHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.SixShooterHit, impulse, PlayerKilledByType.SixShooter);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.SixShooter, interactionStruct.Impulse);
	}
}
