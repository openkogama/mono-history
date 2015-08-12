using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MultiThrowingStarHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse, float damage)
	{
		return new InteractionData(InteractionPackageType.ThrowingStarHit, damage, impulse);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if (component != null)
		{
			component.AddImpulse(interactionStruct.Impulse, suspendImpactDamage: true);
		}
		MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component2 != null)
		{
			component2.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.MultiThrowingStar);
		}
	}
}
