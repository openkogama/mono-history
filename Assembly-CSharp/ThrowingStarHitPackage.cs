using MV.Common;
using MV.WorldObject;

public class ThrowingStarHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.ThrowingStarHit, PlayerKilledByType.ThrowingStar);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.ThrowingStar, interactionStruct.Impulse);
	}
}
