using UnityEngine;

public class SkinnedMeshOptimizer : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer skinnedMesh;

	[SerializeField]
	private MeshRenderer mesh;

	private SkinnedMeshOptimizeManager.SkinnedMeshOptimizationData optimizationData = default;

	private bool isEnabled = true;

	public void TurnOffMesh()
	{
		mesh.enabled = false;
	}

	public void DisableOptimizer()
	{
		isEnabled = false;
		((AvatarLocal)MVGameControllerBase.WOCM.AvatarLocal.Avatar).SkinnedMeshOptimizeManager.RemoveoptimizationData(optimizationData);
	}

	private void Start()
	{
		optimizationData.skinnedMesh = skinnedMesh;
		optimizationData.mesh = mesh;
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
