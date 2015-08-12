using System;
using System.Collections.Generic;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
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
				MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(overlapResult.woId);
				if (worldObjectClient is MVCubeModelFineGrainedTerrain)
				{
					MVCubeModelFineGrainedTerrain mVCubeModelFineGrainedTerrain = (MVCubeModelFineGrainedTerrain)worldObjectClient;
					IntVector[] localCubePos = overlapResult.localCubePos;
					foreach (IntVector intVector in localCubePos)
					{
						Cube cube = mVCubeModelFineGrainedTerrain.GetCube(intVector);
						if (!(cube == null))
						{
							float toughness = MVGameController.Game.MaterialRepository.GetMaterial(cube.FaceMaterials[0]).physicalProperties.toughness;
							if (toughness != 0f)
							{
								MVGameController.Game.World.RuntimeEventManager.SendRuntimeEvent(new SingleCubeFineGrainedEvent(intVector));
								mVCubeModelFineGrainedTerrain.RemoveCubeNetworkUpdate(intVector);
							}
							else
							{
								mVCubeModelFineGrainedTerrain.RemoveCube(intVector);
								mVCubeModelFineGrainedTerrain.HandleDelta();
							}
						}
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
					Debug.LogWarning("This happens due to error in MVElipsoid overlapCheck caused by multiple chunks in cube model.");
				}
				else
				{
					dictionary.Add(item.woId, item);
				}
			}
			return dictionary;
		}
	}

	protected MVGroundState groundState = new MVGroundState();

	protected MVCollisionFlags collisionFlags;

	protected float weight = 1f;

	protected float density = 1f;

	protected bool isPlayerControlled = true;

	private List<Vector3> impulseVectors = new List<Vector3>();

	public abstract bool Grounded { get; }

	public abstract Vector3 Velocity { get; }

	public abstract bool IsMovementLocked { get; set; }

	public bool IsPlayerControlled
	{
		get
		{
			return isPlayerControlled;
		}
		set
		{
			isPlayerControlled = value;
		}
	}

	public void AddImpulse(Vector3 impulse, bool suspendImpactDamage = false)
	{
		if (enabled)
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

	protected void Init()
	{
		MVGroundState mVGroundState = groundState;
		mVGroundState.OnGroundChange = (Action<GroundChange>)Delegate.Combine(mVGroundState.OnGroundChange, new Action<GroundChange>(HandleGroundStateChange));
	}

	private void HandleGroundStateChange(GroundChange groundChange)
	{
		if (isPlayerControlled)
		{
			switch (groundChange)
			{
			case GroundChange.FromGroundedToAir:
				GameSessionCounters.SetCount(GameSessionCounterType.Grounded, 0);
				break;
			case GroundChange.FromAirToGrounded:
				GameSessionCounters.SetCount(GameSessionCounterType.Grounded, 1);
				break;
			}
		}
	}

	protected Vector3 GetImpulse(Vector3 velocity, MVInteractableBase interactableLocal)
	{
		if (impulseVectors.Count == 0)
		{
			return velocity;
		}
		Vector3 zero = Vector3.zero;
		foreach (Vector3 impulseVector in impulseVectors)
		{
			zero += impulseVector;
		}
		zero *= 0.02f / interactableLocal.HandleModifierEffect(AvatarModifierEffect.Weight, weight);
		impulseVectors.Clear();
		return velocity + zero;
	}

	protected static Vector3 VelocityDamping(Vector3 velocity, float defaultDampning, MVInteractableBase interactableLocal)
	{
		float num = interactableLocal.HandleModifierEffect(AvatarModifierEffect.VelocityDamping, 1f);
		velocity -= (velocity - num * velocity) * (Time.fixedDeltaTime / 0.02f);
		return velocity;
	}

	protected static Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		Vector3 lhs = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(lhs, groundNormal).normalized * hVelocity.magnitude;
	}

	protected Vector3 ApplyGravity(Vector3 velocity, Vector3 velocityPrevFrame, MVInteractableBase interactableLocal)
	{
		velocity.y = velocityPrevFrame.y - (float)MVPhysics.Gravity * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Density, density) * Time.deltaTime;
		return velocity;
	}

	protected abstract void SuspendImpactDamage();
}
