using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshOptimizeManager : MonoBehaviour
{
	public struct SkinnedMeshOptimizationData
	{
		public SkinnedMeshRenderer skinnedMesh;

		public MeshRenderer mesh;
	}

	private List<SkinnedMeshOptimizationData> optimizationDataList = new List<SkinnedMeshOptimizationData>();

	private const int allowedSkinnedMeshAmount = 5;

	public void AddOptimizationData(SkinnedMeshOptimizationData optimizationData)
	{
		optimizationDataList.Add(optimizationData);
	}

	public void RemoveoptimizationData(SkinnedMeshOptimizationData optimizationData)
	{
		if (optimizationDataList.Contains(optimizationData))
		{
			optimizationDataList.Remove(optimizationData);
		}
	}

	private void Update()
	{
		if (optimizationDataList.Count <= 0)
		{
			return;
		}
		List<SkinnedMeshOptimizationData> list = new List<SkinnedMeshOptimizationData>();
		List<SkinnedMeshOptimizationData> list2 = new List<SkinnedMeshOptimizationData>();
		for (int i = 0; i < optimizationDataList.Count; i++)
		{
			if (optimizationDataList[i].skinnedMesh.isVisible || optimizationDataList[i].mesh.isVisible)
			{
				int index;
				if (list.Count < 5)
				{
					list.Add(optimizationDataList[i]);
				}
				else if (IsNewMeshCloser(optimizationDataList[i], list, out index))
				{
					list2.Add(list[index]);
					list.RemoveAt(index);
					list.Add(optimizationDataList[i]);
				}
				else
				{
					list2.Add(optimizationDataList[i]);
				}
				optimizationDataList[i].skinnedMesh.enabled = false;
				optimizationDataList[i].mesh.enabled = false;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].skinnedMesh.enabled = true;
		}
		for (int k = 0; k < list2.Count; k++)
		{
			list2[k].mesh.enabled = true;
		}
	}

	private bool IsNewMeshCloser(SkinnedMeshOptimizationData newMesh, List<SkinnedMeshOptimizationData> oldMeshes, out int index)
	{
		index = -1;
		float sqrMagnitude = (newMesh.skinnedMesh.transform.position - MVGameControllerBase.WOCM.AvatarLocal.Transform.position).sqrMagnitude;
		for (int i = 0; i < oldMeshes.Count; i++)
		{
			float sqrMagnitude2 = (oldMeshes[i].skinnedMesh.transform.position - MVGameControllerBase.WOCM.AvatarLocal.Transform.position).sqrMagnitude;
			if (sqrMagnitude < sqrMagnitude2)
			{
				index = i;
				return true;
			}
		}
		return false;
	}
}
