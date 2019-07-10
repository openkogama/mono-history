using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshOptimizeManager : IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
{
	public struct SkinnedMeshOptimizationData
	{
		public List<SkinnedMeshRenderer> skinnedMesh;

		public List<MeshRenderer> mesh;
	}

	private List<SkinnedMeshOptimizationData> optimizationDataList = new List<SkinnedMeshOptimizationData>();

	private const int allowedSkinnedMeshAmount = 5;

	public void AddOptimizationData(SkinnedMeshOptimizationData optimizationData)
	{
		if (optimizationDataList.Count == 0)
		{
			UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		}
		optimizationDataList.Add(optimizationData);
	}

	public void RemoveoptimizationData(SkinnedMeshOptimizationData optimizationData)
	{
		if (optimizationDataList.Contains(optimizationData))
		{
			optimizationDataList.Remove(optimizationData);
		}
		if (optimizationDataList.Count == 0)
		{
			UpdateController.RemoveUpdateObject(this);
		}
	}

	public void UpdateControllerUpdate()
	{
		if (optimizationDataList.Count <= 0)
		{
			return;
		}
		List<SkinnedMeshOptimizationData> list = new List<SkinnedMeshOptimizationData>();
		List<SkinnedMeshOptimizationData> list2 = new List<SkinnedMeshOptimizationData>();
		for (int i = 0; i < optimizationDataList.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < optimizationDataList[i].skinnedMesh.Count; j++)
			{
				if (optimizationDataList[i].skinnedMesh[j].isVisible || optimizationDataList[i].mesh[j].isVisible)
				{
					flag = true;
				}
			}
			if (flag)
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
				for (int k = 0; k < optimizationDataList[i].skinnedMesh.Count; k++)
				{
					optimizationDataList[i].skinnedMesh[k].enabled = false;
					optimizationDataList[i].mesh[k].enabled = false;
				}
			}
		}
		for (int l = 0; l < list.Count; l++)
		{
			for (int m = 0; m < list[l].skinnedMesh.Count; m++)
			{
				list[l].skinnedMesh[m].enabled = true;
			}
		}
		for (int n = 0; n < list2.Count; n++)
		{
			for (int num = 0; num < list2[n].mesh.Count; num++)
			{
				list2[n].mesh[num].enabled = true;
			}
		}
	}

	public void UpdateControllerFixedUpdate()
	{
	}

	private bool IsNewMeshCloser(SkinnedMeshOptimizationData newMesh, List<SkinnedMeshOptimizationData> oldMeshes, out int index)
	{
		index = -1;
		float sqrMagnitude = (newMesh.skinnedMesh[0].transform.position - MVGameControllerBase.SpawnRoleDataMediatorLocal.Position).sqrMagnitude;
		for (int i = 0; i < oldMeshes.Count; i++)
		{
			float sqrMagnitude2 = (oldMeshes[i].skinnedMesh[0].transform.position - MVGameControllerBase.SpawnRoleDataMediatorLocal.Position).sqrMagnitude;
			if (sqrMagnitude < sqrMagnitude2)
			{
				index = i;
				return true;
			}
		}
		return false;
	}
}
