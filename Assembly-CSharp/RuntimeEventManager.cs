using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class RuntimeEventManager
{
	private class AccumulatedCubeDamages : IUpdatecontrollerSubscriber
	{
		private class AccumulatedCubeDamage
		{
			private const float intervalInSecondsBeforeReset = 5f;

			private float lastReceivedDamage = Time.time;

			private float damage;

			public bool Expired => Time.time - lastReceivedDamage > 5f;

			public float AddDamage(float damageDelta)
			{
				lastReceivedDamage = Time.time;
				damage += damageDelta;
				return damage;
			}
		}

		private Dictionary<IntVector, AccumulatedCubeDamage> accumulatedCubeDamages = new Dictionary<IntVector, AccumulatedCubeDamage>();

		public AccumulatedCubeDamages()
		{
			UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		}

		public float AddDamageToCube(float damage, IntVector position)
		{
			if (!accumulatedCubeDamages.ContainsKey(position))
			{
				accumulatedCubeDamages.Add(position, new AccumulatedCubeDamage());
			}
			return accumulatedCubeDamages[position].AddDamage(damage);
		}

		public void UpdateControllerUpdate()
		{
			List<IntVector> list = new List<IntVector>();
			foreach (KeyValuePair<IntVector, AccumulatedCubeDamage> accumulatedCubeDamage in accumulatedCubeDamages)
			{
				if (accumulatedCubeDamage.Value.Expired)
				{
					list.Add(accumulatedCubeDamage.Key);
				}
			}
			foreach (IntVector item in list)
			{
				accumulatedCubeDamages.Remove(item);
			}
		}

		public void Clear()
		{
			accumulatedCubeDamages.Clear();
		}

		public void UpdateControllerFixedUpdate()
		{
		}
	}

	protected MVCubeModelPrototypeTerrain cubeModelPrototypeTerrain;

	protected MVCubeModelFineGrainedTerrain cubeModelFineGrainedTerrain;

	private readonly AccumulatedCubeDamages localAccumulatedCubeDamages = new AccumulatedCubeDamages();

	protected bool doEffects;

	public void ResetTerrain()
	{
		cubeModelFineGrainedTerrain.Reset();
		cubeModelPrototypeTerrain.Reset();
		localAccumulatedCubeDamages.Clear();
	}

	public void SendRuntimeEvent(ExplosionEvent explosion)
	{
		if (HandleEvent(explosion))
		{
			MVGameControllerBase.OperationRequests.SendRuntimeEventOperation(explosion);
		}
	}

	public void ExecuteRuntimeEventLocal(ExplosionEvent explosion)
	{
		HandleEvent(explosion);
	}

	public bool SendRemoveOneFineGrainedCube(VoxelHit voxelHit, float damage)
	{
		if (!(MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId) is MVCubeModelBase))
		{
			return false;
		}
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
		CubeBase cubeBase = mVCubeModelBase.GetCubeBase(voxelHit.cubePos);
		if (cubeBase == null)
		{
			IntVector pos = CubeMathFunctions.WorldPosToFineGrainedLocalPos(voxelHit.point, voxelHit.normal);
			cubeBase = cubeModelFineGrainedTerrain.GetCubeBase(pos);
			if (cubeBase == null)
			{
				return false;
			}
		}
		IntVector position = voxelHit.cubePos;
		if (mVCubeModelBase.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain)
		{
			position = CubeMathFunctions.WorldPosToFineGrainedLocalPos(voxelHit.point, voxelHit.normal);
		}
		damage = localAccumulatedCubeDamages.AddDamageToCube(damage, position);
		CubeDamageState cubeDamageState = RemoveCubes.RemoveOneCube.CanRemoveCube(cubeBase, damage, MVGameControllerBase.Game.MaterialRepository.GetMaterialPhysicalProperties);
		switch (cubeDamageState)
		{
		case CubeDamageState.NoDamage:
			return false;
		case CubeDamageState.ReceivedDamage:
			SharedWorldObjectGameplayFunctions.DustEfffect(PrefabPool.Instance.ParticleCubeDust, voxelHit.point, 1f);
			break;
		}
		if (cubeDamageState == CubeDamageState.Destroyed)
		{
			SendRuntimeEvent(new SingleCubeFineGrainedEvent(position));
			return true;
		}
		return true;
	}

	public void SendRuntimeEvent(SingleCubeFineGrainedEvent singleCubeFineGrainedEvent)
	{
		if (HandleEvent(singleCubeFineGrainedEvent))
		{
			if (IsRemovingAddedFineGrainedCube(singleCubeFineGrainedEvent))
			{
				singleCubeFineGrainedEvent.OverrideRuntimeType(RuntimeEventType.FineGrainedSingleCubeRemovedAddedFineGrainedCube);
			}
			MVGameControllerBase.OperationRequests.SendRuntimeEventOperation(singleCubeFineGrainedEvent);
		}
	}

	private bool IsRemovingAddedFineGrainedCube(SingleCubeFineGrainedEvent singleCubeFineGrainedEvent)
	{
		if (singleCubeFineGrainedEvent.RuntimeEventType != RuntimeEventType.FineGrainedSingleCubeRemove)
		{
			return false;
		}
		IntVector intVector = CubeMathFunctions.FromLocalPosToLocalPos(singleCubeFineGrainedEvent.Position, cubeModelPrototypeTerrain, cubeModelFineGrainedTerrain);
		if (!cubeModelPrototypeTerrain.RemovedCubesContainsKey(intVector))
		{
			return true;
		}
		return false;
	}

	protected bool HandleEvent(SingleCubeFineGrainedEvent singleCubeFineGrainedEvent)
	{
		if (singleCubeFineGrainedEvent.RuntimeEventType == RuntimeEventType.FineGrainedSingleCubeAdd)
		{
			cubeModelFineGrainedTerrain.AddCubeNetworkUpdate(singleCubeFineGrainedEvent.Position, new CubeBase(singleCubeFineGrainedEvent.Material));
		}
		else if ((singleCubeFineGrainedEvent.RuntimeEventType == RuntimeEventType.FineGrainedSingleCubeRemove || singleCubeFineGrainedEvent.RuntimeEventType == RuntimeEventType.FineGrainedSingleCubeRemovedAddedFineGrainedCube) && RemoveCubes.RemoveOneCube.HandleRemoveOneCube(singleCubeFineGrainedEvent.Position, cubeModelPrototypeTerrain, cubeModelFineGrainedTerrain) && doEffects)
		{
			SharedWorldObjectGameplayFunctions.DustEfffect(PrefabPool.Instance.ParticleCubeDustDestroyed, CubeMathFunctions.FineGrainedLocalPosToWorldPos(singleCubeFineGrainedEvent.Position), 1f);
		}
		return true;
	}

	protected bool HandleEvent(ExplosionEvent explosion)
	{
		ExplosionEvent.ExplosionValues explosionValuesStruct = explosion.ExplosionValuesStruct;
		bool flag = RemoveCubes.RemoveCubesWithinRadius.HandleRemoveCubes(cubeModelPrototypeTerrain, explosionValuesStruct.Radius, explosion.Position, explosionValuesStruct.CenterDamage, explosionValuesStruct.DamageFallOffType, cubeModelFineGrainedTerrain, MVGameControllerBase.Game.MaterialRepository.GetMaterialPhysicalProperties);
		if (flag && doEffects)
		{
			SharedWorldObjectGameplayFunctions.DustEfffect(PrefabPool.Instance.ParticleCubeDust, CubeMathFunctions.FineGrainedLocalPosToWorldPos(explosion.Position), Mathf.Max(1.5f, explosionValuesStruct.Radius));
		}
		return flag;
	}
}
