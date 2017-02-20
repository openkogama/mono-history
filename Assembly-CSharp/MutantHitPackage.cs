using MV.Common;
using MV.WorldObject;

public class MutantHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.MutantHit);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.Mutant);
	}
}
