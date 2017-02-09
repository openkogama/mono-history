using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class CenterGunHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.CenterGun, impulse, PlayerKilledByType.CenterGun);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component != null)
		{
			component.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.CenterGun);
		}
		MVWorldObjectClient worldObjectClient2 = MVGameControllerBase.WOCM.GetWorldObjectClient(shooter.Avatar.Id);
		if (worldObjectClient2 != null)
		{
			MVRigidBody component2 = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
			if (component2 != null)
			{
				component2.AddImpulse(shooter, interactionStruct.Impulse, suspendImpactDamage: true);
			}
		}
	}
}
