using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class CenterGunHitPackage : InteractionPackage
{
	private float impulseStrength = 700f;

	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.CenterGun);
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
			Vector3 normalized = (worldObjectClient.GetTargetPosition() - worldObjectClient2.WorldPosition).normalized;
			MVRigidBody component2 = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
			if (component2 != null)
			{
				component2.AddImpulse(normalized * impulseStrength, suspendImpactDamage: true);
			}
			Vector3 vector = -normalized;
			vector.y = 0f;
			vector.Normalize();
			float num = 0.5f;
			float num2 = num * Mathf.Tan((float)Math.PI / 180f * Vector3.Angle(vector, -normalized));
			vector *= num;
			if (0f - normalized.y > 0f)
			{
				vector.y = num2;
			}
			else
			{
				vector.y = 0f - num2;
			}
			Vector3 position = vector + Vector3.up + worldObjectClient.GetTargetPosition();
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normalized);
			UnityEngine.Object.Instantiate(PrefabPool.Instance.ParticleBlood, position, rotation);
		}
	}
}
