using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class RailgunHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.RailGunHit);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if ((Object)(object)component != (Object)null)
		{
			component.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.RailGun);
		}
	}
}
