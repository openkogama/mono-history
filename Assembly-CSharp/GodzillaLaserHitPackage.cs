using MV.Common;
using MV.WorldObject;

public class GodzillaLaserHitPackage : InteractionPackage
{
	private const float baseDamage = 9f;

	public static InteractionData Create(float damageMultiplier)
	{
		return new InteractionData(InteractionPackageType.GodzillaLaserHit, 9f * damageMultiplier, PlayerKilledByType.GodzillaLaser);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		HandlePackage(worldObjectClient, shooter, interactionStruct.Damage, PlayerKilledByType.GodzillaLaser);
	}
}
