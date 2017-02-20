using MV.WorldObject;

public class GodzillaLaserBurnPackageM : GodzillaLaserBurnPackage
{
	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, AvatarModifierPackageType.GodzillaLaserBurnM);
	}
}
