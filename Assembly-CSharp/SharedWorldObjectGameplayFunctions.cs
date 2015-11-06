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
						Debug.Log("Doing local destruction");
						if (MVGameControllerBase.Game.World.RuntimeEventManager != null)
						{
							MVGameControllerBase.Game.World.RuntimeEventManager.ExecuteRuntimeEventLocal(explosionEvent);
						}
					}
					else
					{
						Debug.Log("Sending destuction to all");
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

		public static void Explode(string particleResource, Vector3 position, float damageValue, float damageRadius, float shockwaveAcceleration, bool local, ExplosionEvent explosionEvent)
		{
			Explode(particleResource, position, damageValue, damageRadius, shockwaveAcceleration, local, explosionEvent, new HashSet<int>());
		}

		public static void Explode(string particleResource, Vector3 position, float damageValue, float damageRadius, float shockwaveAcceleration, bool local, ExplosionEvent explosionEvent, HashSet<int> ignoreIDs)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(particleResource), position, Quaternion.identity);
			Detonator component = gameObject.GetComponent<Detonator>();
			if (component == null)
			{
				Debug.LogError("Detonator script not found on prefab");
				return;
			}
			component.size = damageRadius;
			ApplyProximityDamage(position, damageValue, damageRadius, shockwaveAcceleration, local, explosionEvent, ignoreIDs);
		}
	}

	public static void DustEfffect(string particleResource, Vector3 position, float radius)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(particleResource), position, Quaternion.identity);
		Detonator component = gameObject.GetComponent<Detonator>();
		if (component == null)
		{
			Debug.LogError("Detonator script not found on prefab");
		}
		else
		{
			component.size = radius;
		}
	}
}
