using MV.WorldObject;

public class GodzillaLaserBurnPackageXL : GodzillaLaserBurnPackage
{
	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, AvatarModifierPackageType.GodzillaLaserBurnXL);
	}
}
