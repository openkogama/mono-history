using UnityEngine;

public class SkinnedMeshOptimizer : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer skinnedMesh;

	[SerializeField]
	private MeshRenderer mesh;

	private SkinnedMeshOptimizeManager.SkinnedMeshOptimizationData optimizationData = default;

	public void TurnOffMesh()
	{
		mesh.enabled = false;
	}

	private void Start()
	{
		optimizationData.skinnedMesh = skinnedMesh;
		optimizationData.mesh = mesh;
		((AvatarLocal)MVGameControllerBase.WOCM.AvatarLocal.Avatar).SkinnedMeshOptimizeManager.AddOptimizationData(optimizationData);
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			((AvatarLocal)MVGameControllerBase.WOCM.AvatarLocal.Avatar).SkinnedMeshOptimizeManager.RemoveoptimizationData(optimizationData);
		}
	}
}
