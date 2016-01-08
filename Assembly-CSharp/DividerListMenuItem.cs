using UnityEngine;

public class DividerListMenuItem : ListMenuItem
{
	protected override void Initialize()
	{
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = PrefabPool.Instance.ListMenuDividerMaterial;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		UXUtils.BuildPlaneMesh(meshFilter.mesh, width, GetHeight(), "ListMenuDividerMesh");
	}

	public override float GetHeight()
	{
		return 0.5f;
	}

	protected override void UpdateMouseOver()
	{
	}
}
