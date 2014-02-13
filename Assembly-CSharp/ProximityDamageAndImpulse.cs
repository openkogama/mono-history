using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ProximityDamageAndImpulse : InteractionPackage
{
	public static InteractionData Create(float damage, Vector3 impulse, PlayerKilledByType playerKilledByType = PlayerKilledByType.None)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return new InteractionData(InteractionPackageType.ProximityDamageAndImpulse, damage, impulse, playerKilledByType);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (interactionStruct.Impulse != Vector3.zero)
		{
			MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
			if ((Object)(object)component != (Object)null)
			{
				component.AddImpulse(interactionStruct.Impulse, suspendImpactDamage: true);
			}
		}
		if (interactionStruct.Damage != 0f)
		{
			MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
			if ((Object)(object)component2 != (Object)null)
			{
				component2.TakeDamage(interactionStruct.Damage, shooter, interactionStruct.PlayerKilledByType);
			}
		}
	}
}
