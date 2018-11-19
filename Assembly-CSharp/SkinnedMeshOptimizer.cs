using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshOptimizer : MonoBehaviour
{
	[Serializable]
	private struct MeshData
	{
		public SkinnedMeshRenderer skinnedMesh;

		public MeshRenderer mesh;
	}

	[SerializeField]
	private List<MeshData> meshData;

	private SkinnedMeshOptimizeManager.SkinnedMeshOptimizationData optimizationData = default;

	private bool isEnabled = true;

	public void TurnOffMesh()
	{
		for (int i = 0; i < meshData.Count; i++)
		{
			meshData[i].mesh.enabled = false;
		}
	}

	public void DisableOptimizer()
	{
		isEnabled = false;
		((AvatarLocal)MVGameControllerBase.WOCM.AvatarLocal.Avatar).SkinnedMeshOptimizeManager.RemoveoptimizationData(optimizationData);
	}

	private void Start()
	{
		List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>();
		List<MeshRenderer> list2 = new List<MeshRenderer>();
		for (int i = 0; i < meshData.Count; i++)
		{
			list.Add(meshData[i].skinnedMesh);
			list2.Add(meshData[i].mesh);
		}
		optimizationData.skinnedMesh = list;
		optimizationData.mesh = list2;
		if (isEnabled)
		{
			((AvatarLocal)MVGameControllerBase.WOCM.AvatarLocal.Avatar).SkinnedMeshOptimizeManager.AddOptimizationData(optimizationData);
		}
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null && isEnabled)
		{
			((AvatarLocal)MVGameControllerBase.WOCM.AvatarLocal.Avatar).SkinnedMeshOptimizeManager.RemoveoptimizationData(optimizationData);
		}
	}
}
