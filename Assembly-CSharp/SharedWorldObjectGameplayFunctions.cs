using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class SharedWorldObjectGameplayFunctions
{
	public class Explosion
	{
		private static void ApplyProximityDamage(Vector3 position, float damageValue, float damageRadius, float shockwaveAcceleration, HashSet<int> ignoreIDs)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			Collider[] array = Physics.OverlapSphere(position, damageRadius, 1 << LayerMask.NameToLayer("Player"));
			Collider[] array2 = array;
			foreach (Collider val in array2)
			{
				MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)val).transform);
				if (mVObject == null || ignoreIDs.Contains(mVObject.Id))
				{
					continue;
				}
				float num = Vector3.Distance(((Component)val).collider.ClosestPointOnBounds(position), position);
				if (num <= damageRadius)
				{
					InteractionDataHandlerBase component = mVObject.GameObject.GetComponent<InteractionDataHandlerBase>();
					if (!((Object)(object)component == (Object)null))
					{
						float num2 = 1f - num / damageRadius;
						float damage = damageValue * num2;
						Vector3 val2 = mVObject.GameObject.transform.position - position;
						Vector3 impulse = val2.normalized * num2 * shockwaveAcceleration;
						InteractionData interaction = ProximityDamageAndImpulse.Create(damage, impulse, PlayerKilledByType.Explosive);
						component.HandleInteraction(interaction, interactionIsLocal: true);
					}
				}
			}
		}

		public void Explode(Vector3 position, float damageValue, float damageRadius, float shockwaveAcceleration)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			Explode(position, damageValue, damageRadius, shockwaveAcceleration, new HashSet<int>());
		}

		public void Explode(Vector3 position, float damageValue, float damageRadius, float shockwaveAcceleration, HashSet<int> ignoreIDs)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected Obj, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = (GameObject)Object.Instantiate(Resources.Load("ParticleFX/Explosion"), position, Quaternion.identity);
			Detonator component = val.GetComponent<Detonator>();
			if ((Object)(object)component == (Object)null)
			{
				Debug.LogError((object)"Detonator script not found on prefab");
				return;
			}
			component.size = damageRadius;
			ApplyProximityDamage(position, damageValue, damageRadius, shockwaveAcceleration, ignoreIDs);
		}
	}

	public Explosion ExplosionCreator { get; set; }

	public SharedWorldObjectGameplayFunctions()
	{
		ExplosionCreator = new Explosion();
	}
}
