using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ProximityDamageAndImpulse : InteractionPackage
{
	public static InteractionData Create(float damage, Vector3 impulse, PlayerKilledByType playerKilledByType = PlayerKilledByType.None)
	{
		return new InteractionData(InteractionPackageType.ProximityDamageAndImpulse, damage, impulse, playerKilledByType);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		if (interactionStruct.Impulse != Vector3.zero)
		{
			MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
			if (component != null)
			{
				component.AddImpulse(interactionStruct.Impulse, suspendImpactDamage: true);
			}
		}
		if (interactionStruct.Damage != 0f)
		{
			MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
			if (component2 != null)
			{
				component2.TakeDamage(interactionStruct.Damage, shooter, interactionStruct.PlayerKilledByType);
			}
		}
	}
}
