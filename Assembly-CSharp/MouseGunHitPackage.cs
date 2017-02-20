using MV.WorldObject;

public class MouseGunHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.MouseGunHit);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, AvatarModifierPackageType.Shrunken);
	}
}
