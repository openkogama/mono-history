using MV.Common;
using MV.WorldObject;

public class RailgunHitPackage : InteractionPackage
{
	public static InteractionData Create(float damage)
	{
		return new InteractionData(InteractionPackageType.RailGunHit, damage);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component != null)
		{
			component.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.RailGun);
		}
	}
}
