using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class SharedWorldObjectGameplayFunctions
{
	public static class Explosion
	{
		private static void ApplyProximityDamage(Vector3 position, float damageValue, float damageRadius, float shockwaveAcceleration, bool local, ExplosionEvent explosionEvent, HashSet<int> ignoreIDs)
		{
			Collider[] array = Physics.OverlapSphere(position, damageRadius);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collider.transform);
				if (mVObject == null || ignoreIDs.Contains(mVObject.Id))
				{
					continue;
				}
				if (mVObject.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain && explosionEvent != null)
				{
					if (local)
					{
						if (MVGameControllerBase.Game.World.RuntimeEventManager != null)
						{
							MVGameControllerBase.Game.World.RuntimeEventManager.ExecuteRuntimeEventLocal(explosionEvent);
						}
					}
					else
					{
						MVGameControllerBase.Game.World.RuntimeEventManager.SendRuntimeEvent(explosionEvent);
					}
				}
				float num = Vector3.Distance(collider.GetComponent<Collider>().ClosestPointOnBounds(position), position);
				if (num <= damageRadius)
				{
					InteractionDataHandlerBase component = mVObject.GameObject.GetComponent<InteractionDataHandlerBase>();
					if (!(component == null))
					{
						float num2 = 1f - num / damageRadius;
						float damage = damageValue * num2;
						Vector3 impulse = (mVObject.GameObject.transform.position - position).normalized * num2 * shockwaveAcceleration;
						InteractionData interaction = ProximityDamageAndImpulse.Create(damage, impulse, PlayerKilledByType.Explosive);
						component.HandleInteraction(interaction, local);
					}
				}
			}
		}

		public static void Explode(GameObject particlePrefab, Vector3 position, float damageValue, float damageRadius, float shockwaveAcceleration, bool local, ExplosionEvent explosionEvent, HashSet<int> ignoreIDs)
		{
			GameObject gameObject = Object.Instantiate(particlePrefab, position, Quaternion.identity) as GameObject;
			Detonator component = gameObject.GetComponent<Detonator>();
			component.size = damageRadius;
			ApplyProximityDamage(position, damageValue, damageRadius, shockwaveAcceleration, local, explosionEvent, ignoreIDs);
		}
	}

	public static void DustEfffect(GameObject particlePrefab, Vector3 position, float radius)
	{
		GameObject gameObject = Object.Instantiate(particlePrefab, position, Quaternion.identity) as GameObject;
		Detonator component = gameObject.GetComponent<Detonator>();
		component.size = radius;
	}
}
