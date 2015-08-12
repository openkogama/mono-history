using MV.WorldObject;
using UnityEngine;

public class ImpulseHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.ImpulseGunHit, impulse);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		Debug.Log(interactionStruct);
		MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if (component != null)
		{
			component.AddImpulse(interactionStruct.Impulse, suspendImpactDamage: true);
		}
		MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component2 != null)
		{
			component2.AddModifier(AvatarModifierPackageType.NoFriction);
		}
	}
}
