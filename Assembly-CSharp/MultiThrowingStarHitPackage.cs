using MV.Common;
using MV.WorldObject;

public class MultiThrowingStarHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.MultiThrowingStarHit, PlayerKilledByType.MultiThrowingStar);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.MultiThrowingStar, interactionStruct.Impulse);
	}
}
