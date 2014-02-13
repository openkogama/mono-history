using MV.WorldObject;
using UnityEngine;

public class ImpulseHitPackage : InteractionPackage
{
	public static InteractionData Create(Vector3 impulse)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return new InteractionData(InteractionPackageType.ImpulseGunHit, impulse);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)interactionStruct);
		MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if ((Object)(object)component != (Object)null)
		{
			component.AddImpulse(interactionStruct.Impulse, suspendImpactDamage: true);
		}
		MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if ((Object)(object)component2 != (Object)null)
		{
			component2.AddModifier(AvatarModifierPackageType.NoFriction);
		}
	}
}
