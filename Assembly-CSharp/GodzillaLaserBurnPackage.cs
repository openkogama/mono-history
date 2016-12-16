using MV.WorldObject;
using UnityEngine;

public class GodzillaLaserBurnPackage : InteractionPackage
{
	public static InteractionData Create(GodzillaModifier.LaserBurnInteractionPackageType godzillaLaserType)
	{
		return new InteractionData(GodzillaModifier.ToInteractionPackageType(godzillaLaserType));
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		Debug.LogError("Trying to parse base GodzillaLaserBurnPackage");
	}
}
