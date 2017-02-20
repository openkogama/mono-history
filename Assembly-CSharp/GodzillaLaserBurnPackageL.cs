using MV.WorldObject;

public class GodzillaLaserBurnPackageL : GodzillaLaserBurnPackage
{
	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, AvatarModifierPackageType.GodzillaLaserBurnL);
	}
}
