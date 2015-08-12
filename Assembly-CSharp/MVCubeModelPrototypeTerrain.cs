using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVCubeModelPrototypeTerrain : MVCubeModelBase
{
	private Dictionary<IntVector, CubeBase> removedCubes = new Dictionary<IntVector, CubeBase>();

	private TerrainLODComponent terrainLODComponent;

	public bool RequiresResetToEdit => removedCubes.Count > 0;

	public MVCubeModelPrototypeTerrain(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
		: base(data, worldObjects, prototypes)
	{
		interactionFlags = InteractionFlags.IsTerrain;
		MVGameController.WOCM.UpdateWorldBounds(SharedCubeFunctions.GetAxisAlignedBoundsRecursively(gameObject.transform).Value);
		terrainLODComponent = new TerrainLODComponent(prototypeCubeModel, chunkInstances, new DynamicLODDistance(1f, 600f, 20000), Scale.x, debug: false);
	}

	public void ChangeLODTerrain()
	{
		terrainLODComponent.ChangeLODTerrain();
	}

	public override void Destroy()
	{
		prototypeCubeModel.RemoveInstance(id);
		base.Destroy();
	}

	public override void Select(Color color)
	{
	}

	public override void DeSelect()
	{
	}

	public GameObject GetChunkInstance(IntVector chunkPos)
	{
		return chunkInstances.GetChunk(chunkPos);
	}

	public Vector3 GetRandomCubePos()
	{
		return prototypeCubeModel.GetRandomCubePos(gameObject);
	}

	public override void RemoveCubeNetworkUpdate(IntVector pos)
	{
		if (removedCubes.ContainsKey(pos))
		{
			Debug.LogWarning("This has already been destroyed");
			return;
		}
		removedCubes.Add(pos, GetCubeBase(pos));
		base.RemoveCubeNetworkUpdate(pos);
	}

	public override void Reset()
	{
		base.Reset();
		foreach (KeyValuePair<IntVector, CubeBase> removedCube in removedCubes)
		{
			AddCubeNetworkUpdate(removedCube.Key, removedCube.Value);
		}
		removedCubes.Clear();
	}

	public bool RemovedCubesContainsKey(IntVector intVector)
	{
		return removedCubes.ContainsKey(intVector);
	}
}
