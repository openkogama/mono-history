using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class ImpactState
{
	public class ImpactDamageRuntimeEventType : IComparable
	{
		private readonly float damageThreshold;

		private readonly RuntimeEventType runtimeEventType;

		public float DamageThreshold => damageThreshold;

		public RuntimeEventType RuntimeEventType => runtimeEventType;

		public ImpactDamageRuntimeEventType(float damageThreshold, RuntimeEventType runtimeEventType)
		{
			this.damageThreshold = damageThreshold;
			this.runtimeEventType = runtimeEventType;
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (obj is ImpactDamageRuntimeEventType impactDamageRuntimeEventType)
			{
				return damageThreshold.CompareTo(impactDamageRuntimeEventType.damageThreshold);
			}
			throw new ArgumentException("Object is not a ImpactDamageRuntimeEventType");
		}

		public override string ToString()
		{
			return $"RuntimeEventType: {runtimeEventType}. DamageThreshold {damageThreshold}";
		}
	}

	private class ImpactDestruction
	{
		private List<ImpactDamageRuntimeEventType> impactDamageRuntimeEventTypes = new List<ImpactDamageRuntimeEventType>();

		private float velChangeToDamageConstant = 1.5f;

		public ImpactDestruction(params RuntimeEventType[] explosionEvents)
		{
			foreach (RuntimeEventType runtimeEventType in explosionEvents)
			{
				float centerDamage = ExplosionEvent.GetExplosionValuesStruct(runtimeEventType).CenterDamage;
				impactDamageRuntimeEventTypes.Add(new ImpactDamageRuntimeEventType(centerDamage, runtimeEventType));
			}
			impactDamageRuntimeEventTypes.Sort();
		}

		public void HandleImpactDestruction(float velChangeMagnitude, List<MVControllerColliderHit> moveHits)
		{
			float impactDamage = GetImpactDamage(velChangeMagnitude);
			if (TryGetExplosionEvent(out var runtimeEventType, impactDamage))
			{
				MVControllerColliderHit colliderHit = GetColliderHit(moveHits);
				ExplosionEvent explosion = new ExplosionEvent(runtimeEventType, colliderHit.hit.point, colliderHit.hit.normal);
				MVGameController.Game.World.RuntimeEventManager.SendRuntimeEvent(explosion);
			}
		}

		private MVControllerColliderHit GetColliderHit(List<MVControllerColliderHit> moveHits)
		{
			MVControllerColliderHit colliderHit = default;
			GetMoveHitWithHighestImpactVelocity(ref colliderHit, moveHits);
			return colliderHit;
		}

		private bool TryGetExplosionEvent(out RuntimeEventType runtimeEventType, float damage)
		{
			runtimeEventType = RuntimeEventType.Undefined;
			if (impactDamageRuntimeEventTypes.Count == 0)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < impactDamageRuntimeEventTypes.Count && !(damage < impactDamageRuntimeEventTypes[i].DamageThreshold); i++)
			{
				result = true;
				runtimeEventType = impactDamageRuntimeEventTypes[i].RuntimeEventType;
			}
			return result;
		}

		private float GetImpactDamage(float velChangeMagnitude)
		{
			return velChangeMagnitude * velChangeToDamageConstant / Time.fixedDeltaTime;
		}

		private void GetMoveHitWithHighestImpactVelocity(ref MVControllerColliderHit colliderHit, List<MVControllerColliderHit> moveHits)
		{
			foreach (MVControllerColliderHit moveHit in moveHits)
			{
				if (moveHit.impactVelocity.sqrMagnitude > colliderHit.impactVelocity.sqrMagnitude)
				{
					colliderHit = moveHit;
				}
			}
		}
	}

	private const int suspendImpactDamageFrames = 1;

	public Vector3 prevVelocityChangeVector = Vector3.zero;

	private bool collidedPrevFrame;

	private float averageSoftnessPrevFrame = 1f;

	private float maxAccBeforeDamageDealt = 55f;

	private float impactDamageMultiplier = 4f;

	private float impactDamage;

	private int suspendImpactDamageCounter = 1;

	private ImpactDestruction impactDestruction;

	private List<MVControllerColliderHit> moveHits = new List<MVControllerColliderHit>();

	public float ImpactDamage => impactDamage;

	public ImpactState(params RuntimeEventType[] explosionEvents)
	{
		impactDestruction = new ImpactDestruction(explosionEvents);
	}

	public void SuspendImpactDamage()
	{
		suspendImpactDamageCounter = 1;
		prevVelocityChangeVector = Vector3.zero;
		collidedPrevFrame = false;
	}

	private float CalcImpactDamage(float velocityChange)
	{
		return velocityChange * impactDamageMultiplier / Time.fixedDeltaTime;
	}

	public void HandleMoveHit(MVControllerColliderHit moveHit)
	{
		if (!moveHit.testWithOutMoving)
		{
			moveHits.Add(moveHit);
		}
	}

	public float UpdateImpactState(Vector3 curVelocity, Vector3 prevVelocity, MVInteractableBase interactableLocal)
	{
		Vector3 vector = (curVelocity - prevVelocity) * Time.deltaTime;
		impactDamage = 0f;
		if (suspendImpactDamageCounter > 0)
		{
			suspendImpactDamageCounter--;
			moveHits.Clear();
			return 0f;
		}
		if (collidedPrevFrame)
		{
			Vector3 vector2 = prevVelocityChangeVector + vector;
			float num = maxAccBeforeDamageDealt * Time.deltaTime;
			float magnitude = vector2.magnitude;
			impactDestruction.HandleImpactDestruction(magnitude, moveHits);
			magnitude *= averageSoftnessPrevFrame;
			if (magnitude > num)
			{
				impactDamage = CalcImpactDamage(magnitude);
				Debug.Log(impactDamage);
			}
			prevVelocityChangeVector = Vector3.zero;
		}
		else
		{
			prevVelocityChangeVector = vector;
		}
		if (moveHits.Count > 0)
		{
			collidedPrevFrame = true;
			averageSoftnessPrevFrame = 0f;
			foreach (MVControllerColliderHit moveHit in moveHits)
			{
				averageSoftnessPrevFrame += interactableLocal.HandleModifierEffect(AvatarModifierEffect.Softness, moveHit.material.physicalProperties.softness);
			}
			averageSoftnessPrevFrame /= moveHits.Count;
		}
		else
		{
			collidedPrevFrame = false;
			averageSoftnessPrevFrame = -1f;
		}
		moveHits.Clear();
		return impactDamage;
	}
}
