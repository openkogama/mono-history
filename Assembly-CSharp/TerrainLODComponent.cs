using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class TerrainLODComponent
{
	private class TriangleCounter
	{
		private HashSet<IntVector> enabledChunks = new HashSet<IntVector>();

		public void Add(IntVector localPos)
		{
			enabledChunks.Add(localPos);
		}

		public void Remove(IntVector localPos)
		{
			enabledChunks.Remove(localPos);
		}

		public int GetEnabledTriangleCount(RuntimePrototypeCubeModel prototypeCubeModel)
		{
			int num = 0;
			List<IntVector> list = new List<IntVector>();
			foreach (IntVector enabledChunk in enabledChunks)
			{
				if (prototypeCubeModel.Chunks.ContainsKey(enabledChunk))
				{
					num += prototypeCubeModel.Chunks[enabledChunk].TriangleCount;
				}
				else
				{
					list.Add(enabledChunk);
				}
			}
			foreach (IntVector item in list)
			{
				enabledChunks.Remove(item);
			}
			return num;
		}
	}

	private List<MVTerrainLOD> LODBookkeeping = new List<MVTerrainLOD>();

	private int currentLODPosition;

	private readonly TriangleCounter triangleCounter = new TriangleCounter();

	private readonly ChunkInstances chunkInstances;

	private readonly RuntimePrototypeCubeModel prototypeCubeModel;

	private readonly DynamicLODDistance dynamicLodDistance;

	private bool debug;

	private float scale;

	public TerrainLODComponent(RuntimePrototypeCubeModel prototypeCubeModel, ChunkInstances chunkInstances, DynamicLODDistance dynamicLodDistance, float scale, bool debug)
	{
		this.prototypeCubeModel = prototypeCubeModel;
		this.chunkInstances = chunkInstances;
		this.dynamicLodDistance = dynamicLodDistance;
		this.scale = scale;
		this.debug = debug;
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)chunkInstances)
		{
			IntVector key = item.Key;
			AddToLOD(key);
		}
		chunkInstances.Changed += chunkInstances_Changed;
	}

	public void ChangeLODTerrain()
	{
		if (LODBookkeeping.Count == 0)
		{
			return;
		}
		float num = 100f;
		Vector3 position = MVGameControllerBase.CameraController.transform.position;
		int num2 = Mathf.Max(1, Mathf.RoundToInt(num * Time.deltaTime));
		for (int i = 0; i < num2; i++)
		{
			if (LODBookkeeping.Count == 0)
			{
				return;
			}
			if (currentLODPosition >= LODBookkeeping.Count)
			{
				currentLODPosition = 0;
			}
			MVTerrainLOD value = LODBookkeeping[currentLODPosition];
			if (chunkInstances.TryGetValue(value.localPos, out var gameObject))
			{
				float num3 = Vector3.Distance(LODBookkeeping[currentLODPosition].worldPos, position);
				ChangeLODChunk(gameObject, ref value.localPos, num3, dynamicLodDistance.CurrentRadius);
				if (num3 < dynamicLodDistance.maxRadius)
				{
					triangleCounter.Add(value.localPos);
				}
				else
				{
					triangleCounter.Remove(value.localPos);
				}
				LODBookkeeping[currentLODPosition] = value;
			}
			else
			{
				LODBookkeeping.RemoveAt(currentLODPosition);
			}
			currentLODPosition++;
		}
		int enabledTriangleCount = triangleCounter.GetEnabledTriangleCount(prototypeCubeModel);
		dynamicLodDistance.Update(enabledTriangleCount);
		if (debug)
		{
			Debug.Log("dynamicLodDistance.CurrentRadius" + dynamicLodDistance.CurrentRadius);
			Debug.Log("TriangleCount: " + triangleCounter.GetEnabledTriangleCount(prototypeCubeModel));
		}
	}

	private void chunkInstances_Changed(object sender, ChunkInstancesChanged e)
	{
		if (e.changeType == ChunkInstancesChanged.ChangeType.Added)
		{
			AddToLOD(e.chunkPos);
		}
	}

	private void AddToLOD(IntVector localPos)
	{
		LODBookkeeping.Add(new MVTerrainLOD(localPos, scale * (float)prototypeCubeModel.ChunkSize * new Vector3(localPos.x, localPos.y, localPos.z)));
	}

	private void ChangeLODChunk(ChunkInstances.ChunkInstanceVariables chunk, ref IntVector chunkPosition, float distance, float renderDistance)
	{
		if (distance > renderDistance && chunk.renderer.enabled)
		{
			chunk.renderer.enabled = false;
			prototypeCubeModel.RemoveRefenceFromChunk(ref chunkPosition);
		}
		if (distance < renderDistance && !chunk.renderer.enabled)
		{
			chunk.renderer.enabled = true;
			prototypeCubeModel.AddRefenceToChunk(ref chunkPosition);
		}
	}
}
