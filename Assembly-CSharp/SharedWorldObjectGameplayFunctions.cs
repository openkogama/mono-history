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
			int num = Physics.OverlapSphereNonAlloc(position, damageRadius, CollisionDetectionGlobalBuffers.colliderBuffer);
			for (int i = 0; i < num; i++)
			{
				Collider collider = CollisionDetectionGlobalBuffers.colliderBuffer[i];
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
				float num2 = Vector3.Distance(collider.ClosestPointOnBounds(position), position);
				if (num2 <= damageRadius)
				{
					InteractionDataHandlerBase interactionDataHandlerBase = mVObject.InteractionDataHandlerBase;
					if (!(interactionDataHandlerBase == null))
					{
						float num3 = 1f - num2 / damageRadius;
						float damage = damageValue * num3;
						Vector3 impulse = (mVObject.GameObject.transform.position - position).normalized * num3 * shockwaveAcceleration;
						InteractionData interaction = ProximityDamageAndImpulse.Create(damage, impulse, PlayerKilledByType.Explosive);
						interactionDataHandlerBase.HandleInteraction(interaction, local);
					}
				}
			}
		}

		public static void Explode(ParticleSystem particlePrefab, Vector3 position, float damageValue, float damageRadius, float shockwaveAcceleration, bool local, ExplosionEvent explosionEvent, HashSet<int> ignoreIDs)
		{
			if (DoParticleEffect(position))
			{
				ParticleSystem particleSystem = Object.Instantiate(particlePrefab, position, Quaternion.identity) as ParticleSystem;
				particleSystem.startSize = damageRadius;
			}
			ApplyProximityDamage(position, damageValue, damageRadius, shockwaveAcceleration, local, explosionEvent, ignoreIDs);
		}
	}

	private static bool DoParticleEffect(Vector3 position)
	{
		Vector3 rhs = position - MVGameControllerBase.CameraController.MainCamera.transform.position;
		float sqrMagnitude = rhs.sqrMagnitude;
		rhs.Normalize();
		float num = Vector3.Dot(MVGameControllerBase.CameraController.MainCamera.transform.rotation * Vector3.forward, rhs);
		if (num > 0f && sqrMagnitude < MVGameControllerBase.CameraController.MainCamera.farClipPlane * MVGameControllerBase.CameraController.MainCamera.farClipPlane)
		{
			return true;
		}
		return false;
	}

	public static void DustEfffect(ParticleSystem particlePrefab, Vector3 position, float radius)
	{
		if (DoParticleEffect(position))
		{
			ParticleSystem particleSystem = Object.Instantiate(particlePrefab, position, Quaternion.identity) as ParticleSystem;
			particleSystem.startSize = radius;
		}
	}
}
