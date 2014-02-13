using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public abstract class MVRigidBody : MVComponent
{
	protected class StuckEvaluator
	{
		internal class StuckObject
		{
			private static float timeBeforeStuck = 2f;

			private float stuckTime;

			private MVOverlapResult overlapResult;

			public MVOverlapResult OverlapResult
			{
				set
				{
					overlapResult = value;
				}
			}

			public StuckObject(MVOverlapResult overlapResult)
			{
				this.overlapResult = overlapResult;
				stuckTime = Time.time;
			}

			public bool IsStuckInObject()
			{
				if (Time.time - stuckTime >= timeBeforeStuck)
				{
					bool flag = HandleFineGrained();
					return !flag;
				}
				return false;
			}

			private bool HandleFineGrained()
			{
				MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(overlapResult.woId);
				if (worldObjectClient is MVCubeModelFineGrainedTerrain)
				{
					MVCubeModelFineGrainedTerrain mVCubeModelFineGrainedTerrain = (MVCubeModelFineGrainedTerrain)worldObjectClient;
					IntVector[] localCubePos = overlapResult.localCubePos;
					foreach (IntVector pos in localCubePos)
					{
						mVCubeModelFineGrainedTerrain.RemoveCube(pos);
					}
					if (overlapResult.localCubePos.Length > 0)
					{
						mVCubeModelFineGrainedTerrain.HandleDelta();
					}
					return true;
				}
				return false;
			}
		}

		private Dictionary<int, StuckObject> stuckObjects = new Dictionary<int, StuckObject>();

		private Func<List<MVOverlapResult>> getOverlappingObjects;

		private float updateInterval = 0.2f;

		private float updateTime = Time.time;

		public StuckEvaluator(Func<List<MVOverlapResult>> getOverlappingObjects)
		{
			this.getOverlappingObjects = getOverlappingObjects;
		}

		public bool Update()
		{
			if (Time.time - updateTime < updateInterval && stuckObjects.Count == 0)
			{
				return false;
			}
			updateTime = Time.time;
			Dictionary<int, MVOverlapResult> overlapDictionary = GetOverlapDictionary();
			if (overlapDictionary == null)
			{
				stuckObjects.Clear();
				return false;
			}
			List<int> list = new List<int>();
			foreach (int key in stuckObjects.Keys)
			{
				if (!overlapDictionary.ContainsKey(key))
				{
					list.Add(key);
				}
			}
			foreach (int item in list)
			{
				stuckObjects.Remove(item);
			}
			foreach (KeyValuePair<int, MVOverlapResult> item2 in overlapDictionary)
			{
				if (!stuckObjects.ContainsKey(item2.Key))
				{
					stuckObjects.Add(item2.Key, new StuckObject(item2.Value));
				}
				else
				{
					stuckObjects[item2.Key].OverlapResult = item2.Value;
				}
			}
			overlapDictionary.Clear();
			foreach (StuckObject value in stuckObjects.Values)
			{
				if (value.IsStuckInObject())
				{
					return true;
				}
			}
			return false;
		}

		private Dictionary<int, MVOverlapResult> GetOverlapDictionary()
		{
			List<MVOverlapResult> list = getOverlappingObjects();
			if (list.Count == 0)
			{
				return null;
			}
			Dictionary<int, MVOverlapResult> dictionary = new Dictionary<int, MVOverlapResult>();
			foreach (MVOverlapResult item in list)
			{
				if (dictionary.ContainsKey(item.woId))
				{
					Debug.LogWarning((object)"This happens due to error in MVElipsoid overlapCheck caused by multiple chunks in cube model.");
				}
				else
				{
					dictionary.Add(item.woId, item);
				}
			}
			return dictionary;
		}
	}

	protected MVCollisionFlags collisionFlags;

	protected float weight = 1f;

	protected float density = 1f;

	private List<Vector3> impulseVectors = new List<Vector3>();

	public abstract bool Grounded { get; }

	public abstract Vector3 Velocity { get; }

	public abstract bool IsMovementLocked { get; set; }

	public void AddImpulse(Vector3 impulse, bool suspendImpactDamage = false)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)this).enabled)
		{
			impulseVectors.Add(impulse);
			if (suspendImpactDamage)
			{
				SuspendImpactDamage();
			}
		}
	}

	public virtual void Reset()
	{
		impulseVectors.Clear();
	}

	protected Vector3 GetImpulse(Vector3 velocity, MVInteractableBase interactableLocal)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (impulseVectors.Count == 0)
		{
			return velocity;
		}
		Vector3 val = Vector3.zero;
		foreach (Vector3 impulseVector in impulseVectors)
		{
			val += impulseVector;
		}
		val /= (float)impulseVectors.Count;
		val *= Time.deltaTime * 1f / interactableLocal.HandleModifierEffect(AvatarModifierEffect.Weight, weight);
		impulseVectors.Clear();
		return velocity + val;
	}

	protected static Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Cross(Vector3.up, hVelocity);
		Vector3 val2 = Vector3.Cross(val, groundNormal);
		return val2.normalized * hVelocity.magnitude;
	}

	protected Vector3 ApplyGravity(Vector3 velocity, Vector3 velocityPrevFrame, MVInteractableBase interactableLocal)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		velocity.y = velocityPrevFrame.y - 30f * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Density, density) * Time.deltaTime;
		return velocity;
	}

	protected abstract void SuspendImpactDamage();
}
