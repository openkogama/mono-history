using UnityEngine;

public class DividerListMenuItem : ListMenuItem
{
	protected override void Initialize()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		MeshRenderer val = ((Component)this).gameObject.AddComponent<MeshRenderer>();
		((Renderer)val).material = (Material)Resources.Load("Materials/UX/ListMenu/Divider");
		MeshFilter val2 = ((Component)this).gameObject.AddComponent<MeshFilter>();
		val2.mesh = UXUtils.BuildPlaneMesh(width, GetHeight(), "ListMenuDividerMesh");
	}

	public override float GetHeight()
	{
		return 0.5f;
	}

	protected override void UpdateMouseOver()
	{
	}
}
