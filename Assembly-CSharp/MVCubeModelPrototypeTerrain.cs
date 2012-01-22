using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVCubeModelPrototypeTerrain : MVCubeModelBase
{
	private List<TerrainLOD> LODBookkeeping = new List<TerrainLOD>();

	private int currentLODPosition;

	protected override void CreateMVWOC(bool local)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		base.CreateMVWOC(local);
		interactionFlags = InteractionFlags.IsTerrain;
		MVGameController.Instance.WOCM.UpdateWorldBounds(SharedCubeFunctions.GetAxisAlignedBoundsRecursively(gameObject.transform).Value);
		foreach (KeyValuePair<IntVector, GameObject> chunkInstance in chunkInstances)
		{
			IntVector key = chunkInstance.Key;
			LODBookkeeping.Add(new TerrainLOD(key, Scale.x * (float)CubeModelChunk.ChunkSize * new Vector3((float)key.x, (float)key.y, (float)key.z)));
		}
		MVGameController.Instance.WOCM.Terrain = this;
	}

	public void ChangeLODTerrain()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		float num = 100f;
		Vector3 val = ((Component)MVGameController.Instance.WOCM.WeCamera).transform.position;
		int num2 = Mathf.Max(1, Mathf.RoundToInt(num * Time.deltaTime));
		for (int i = 0; i < num2; i++)
		{
			if (currentLODPosition >= LODBookkeeping.Count)
			{
				currentLODPosition = 0;
			}
			TerrainLOD terrainLOD = LODBookkeeping[currentLODPosition];
			if (chunkInstances.TryGetValue(terrainLOD.localPos, out var value))
			{
				float distance = Vector3.Distance(LODBookkeeping[currentLODPosition].worldPos, val);
				ChangeLODChunk(ref terrainLOD, value, distance);
				LODBookkeeping[currentLODPosition] = terrainLOD;
			}
			else
			{
				LODBookkeeping.RemoveAt(currentLODPosition);
			}
			currentLODPosition++;
		}
	}

	private void ChangeLODChunk(ref TerrainLOD terrainLOD, GameObject chunk, float distance)
	{
		float num = MVQualitySettings.CurrentLodData[terrainLOD.lodId].activateDistance * 2f;
		float num2 = float.PositiveInfinity;
		bool flag = terrainLOD.lodId + 1 < MVQualitySettings.CurrentLodData.Length;
		bool flag2 = terrainLOD.lodId - 1 >= 0;
		if (flag)
		{
			num2 = MVQualitySettings.CurrentLodData[terrainLOD.lodId + 1].activateDistance * 2f;
		}
		if (!(distance < num2) || !(distance >= num))
		{
			if (distance > num2 && flag)
			{
				terrainLOD.lodId++;
				SetLod(terrainLOD, chunk);
			}
			if (distance < num && flag2)
			{
				terrainLOD.lodId--;
				SetLod(terrainLOD, chunk);
			}
		}
	}

	private void SetLod(TerrainLOD terrainLOD, GameObject chunk)
	{
		if (!MVQualitySettings.CurrentLodData[terrainLOD.lodId].isVisible)
		{
			if (chunk.active)
			{
				chunk.SetActiveRecursively(false);
			}
			return;
		}
		if (!chunk.active)
		{
			chunk.SetActiveRecursively(true);
		}
		CubeModelChunk cubeModelChunk = prototypeCubeModel.Chunks[terrainLOD.localPos];
		chunk.GetComponent<MeshFilter>().sharedMesh = cubeModelChunk.GetMeshData(MVQualitySettings.CurrentLodData[terrainLOD.lodId].mipMeshSetting).mesh;
		((Renderer)chunk.GetComponent<MeshRenderer>()).sharedMaterials = cubeModelChunk.GetMeshData(MVQualitySettings.CurrentLodData[terrainLOD.lodId].mipMeshSetting).materials;
	}

	public override void Initialize()
	{
	}

	public override void Destroy()
	{
		prototypeCubeModel.RemoveInstance(id);
	}

	public override void Select(Color color)
	{
	}

	public override void DeSelect()
	{
	}

	public GameObject GetChunkInstance(IntVector chunkPos)
	{
		return chunkInstances[chunkPos];
	}

	public Vector3 GetRandomCubePos()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return prototypeCubeModel.GetRandomCubePos(gameObject);
	}
}
