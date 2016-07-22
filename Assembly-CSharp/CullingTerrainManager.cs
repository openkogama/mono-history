using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class CullingTerrainManager
{
	private readonly ChunkInstances chunkInstances;

	private readonly MVCubeModelBase cubeModelBase;

	private Dictionary<IntVector, CullingSubscriberTerrainChunk> terrainCullingSubscriberBases = new Dictionary<IntVector, CullingSubscriberTerrainChunk>();

	public CullingTerrainManager(ChunkInstances chunkInstances, MVCubeModelBase cubeModelBase)
	{
		this.chunkInstances = chunkInstances;
		this.chunkInstances.Changed += ChunkInstancesOnChanged;
		this.cubeModelBase = cubeModelBase;
		cubeModelBase.ChunksChanged = (Action<HashSet<IntVector>>)Delegate.Combine(cubeModelBase.ChunksChanged, new Action<HashSet<IntVector>>(OnChanged));
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)chunkInstances)
		{
			CreateCullingSubscriber(item.Key, item.Value);
		}
	}

	private void ChunkInstancesOnChanged(object sender, ChunkInstancesChanged chunkInstancesChanged)
	{
		if (chunkInstancesChanged.changeType == ChunkInstancesChanged.ChangeType.Removed)
		{
			terrainCullingSubscriberBases[chunkInstancesChanged.chunkPos].Destroy();
			terrainCullingSubscriberBases.Remove(chunkInstancesChanged.chunkPos);
		}
		if (chunkInstancesChanged.changeType == ChunkInstancesChanged.ChangeType.Added)
		{
			CreateCullingSubscriber(chunkInstancesChanged.chunkPos, chunkInstances.GetChunk(chunkInstancesChanged.chunkPos));
		}
	}

	private void OnChanged(HashSet<IntVector> chunksChanged)
	{
		foreach (IntVector item in chunksChanged)
		{
			if (!terrainCullingSubscriberBases.ContainsKey(item))
			{
				Debug.LogWarning("Changed chunk does not yet exist");
				continue;
			}
			terrainCullingSubscriberBases[item].Setup(chunkInstances.GetChunk(item).renderer.bounds);
			terrainCullingSubscriberBases[item].HandleChange();
		}
	}

	private void CreateCullingSubscriber(IntVector chunkPos, ChunkInstances.ChunkInstanceVariables chunk)
	{
		Bounds bounds = chunk.renderer.bounds;
		CullingSubscriberTerrainChunk value = new CullingSubscriberTerrainChunk(cubeModelBase, chunkPos, bounds);
		terrainCullingSubscriberBases.Add(chunkPos, value);
	}

	public void Clear()
	{
		foreach (KeyValuePair<IntVector, CullingSubscriberTerrainChunk> terrainCullingSubscriberBasis in terrainCullingSubscriberBases)
		{
			terrainCullingSubscriberBasis.Value.Destroy();
		}
		terrainCullingSubscriberBases.Clear();
	}
}
