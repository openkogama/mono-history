using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class AdvancedGhostBodyRotateWeaponPackage : InteractionPackage
{
	public static InteractionData Create(float damage, Vector3 impulse)
	{
		return new InteractionData(InteractionPackageType.AdvancedGhostBodyRotateWeaponPackage, damage, impulse);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if (component != null)
		{
			component.AddImpulse(interactionStruct.Impulse);
		}
		MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (component2 != null)
		{
			component2.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.AdvancedGhost);
		}
	}
}
