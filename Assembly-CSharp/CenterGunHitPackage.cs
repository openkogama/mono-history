using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class CenterGunHitPackage : InteractionPackage
{
	private float impulseStrength = 700f;

	private string particleHitResource = "ParticleFX/Blood";

	public static InteractionData Create()
	{
		return new InteractionData(InteractionPackageType.CenterGun);
	}

	public override void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		MVInteractableBase component = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if ((Object)(object)component != (Object)null)
		{
			component.TakeDamage(interactionStruct.Damage, shooter, PlayerKilledByType.CenterGun);
		}
		MVWorldObjectClient worldObjectClient2 = MVGameController.Instance.WOCM.GetWorldObjectClient(shooter.Avatar.Id);
		if (worldObjectClient2 != null)
		{
			Vector3 val = worldObjectClient.GetTargetPosition() - worldObjectClient2.WorldPosition;
			Vector3 normalized = val.normalized;
			MVRigidBody component2 = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
			if ((Object)(object)component2 != (Object)null)
			{
				component2.AddImpulse(normalized * impulseStrength, suspendImpactDamage: true);
			}
			Vector3 val2 = -normalized;
			val2.y = 0f;
			val2.Normalize();
			float num = 0.5f;
			float num2 = num * Mathf.Tan((float)Math.PI / 180f * Vector3.Angle(val2, -normalized));
			val2 *= num;
			if (0f - normalized.y > 0f)
			{
				val2.y = num2;
			}
			else
			{
				val2.y = 0f - num2;
			}
			Vector3 val3 = val2 + Vector3.up + worldObjectClient.GetTargetPosition();
			Quaternion val4 = Quaternion.FromToRotation(Vector3.up, normalized);
			Object.Instantiate(Resources.Load(particleHitResource), val3, val4);
		}
	}
}
