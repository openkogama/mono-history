using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class DoubleSixShooterHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.DoubleSixShooterHit, impulse, PlayerKilledByType.DoubleSixShooter);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.DoubleSixShooter, interactionStruct.Impulse);
	}
}
