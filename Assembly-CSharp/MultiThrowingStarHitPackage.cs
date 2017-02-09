using MV.Common;
using MV.WorldObject;

public class MultiThrowingStarHitPackage : InteractionPackage
{
	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.MultiThrowingStarHit, PlayerKilledByType.MultiThrowingStar);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if (component != null)
		{
			component.AddImpulse(shooter, interactionStruct.Impulse, suspendImpactDamage: true);
		}
		MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component2 != null)
		{
			component2.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.MultiThrowingStar);
		}
	}
}
