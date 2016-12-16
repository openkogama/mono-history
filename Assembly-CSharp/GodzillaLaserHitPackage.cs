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
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component != null)
		{
			component.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.GodzillaLaser);
		}
	}
}
