using UnityEngine;

public class DividerListMenuItem : ListMenuItem
{
	protected override void Initialize()
	{
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = (Material)Resources.Load("Materials/UX/ListMenu/Divider");
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
