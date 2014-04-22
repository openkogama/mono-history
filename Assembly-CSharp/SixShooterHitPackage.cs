using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class SixShooterHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse, float damage)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return new InteractionData(InteractionPackageType.SixShooterHit, damage, impulse);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if ((Object)(object)component != (Object)null)
		{
			component.AddImpulse(interactionStruct.Impulse, suspendImpactDamage: true);
		}
		MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if ((Object)(object)component2 != (Object)null)
		{
			component2.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.SixShooter);
		}
	}
}
