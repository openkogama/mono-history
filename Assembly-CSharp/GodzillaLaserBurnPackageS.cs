using MV.WorldObject;

public class GodzillaLaserBurnPackageS : GodzillaLaserBurnPackage
{
	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, AvatarModifierPackageType.GodzillaLaserBurnS);
	}
}
