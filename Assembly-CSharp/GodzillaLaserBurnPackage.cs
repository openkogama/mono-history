using MV.WorldObject;

public abstract class GodzillaLaserBurnPackage : InteractionPackage
{
	public static InteractionData Create(GodzillaModifier.LaserBurnInteractionPackageType godzillaLaserType)
	{
		return new InteractionData(GodzillaModifier.ToInteractionPackageType(godzillaLaserType));
	}
}
