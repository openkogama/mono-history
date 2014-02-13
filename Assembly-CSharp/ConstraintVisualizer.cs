using System.Collections.Generic;
using UnityEngine;

public class ConstraintVisualizer : MonoBehaviour
{
	private IModelingConstraint constraint;

	public void Init(MVCubeModelBase targetCubeModel, IModelingConstraint constraint, string layer = "UIItems")
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)("Init constraint for " + targetCubeModel.Id));
		this.constraint = constraint;
		((Component)this).gameObject.layer = LayerMask.NameToLayer(layer);
		((Component)this).transform.rotation = targetCubeModel.WorldRotation;
		((Component)this).transform.localScale = targetCubeModel.Scale;
		((Component)this).transform.parent = targetCubeModel.GameObject.transform;
		if (constraint is ModelingDynamicBoxConstraint)
		{
			ModelingDynamicBoxConstraint modelingDynamicBoxConstraint = (ModelingDynamicBoxConstraint)constraint;
			((Component)this).transform.position = modelingDynamicBoxConstraint.Center;
			modelingDynamicBoxConstraint.BoxChanged += Constraint_BoxChanged;
		}
		else
		{
			((Component)this).transform.position = targetCubeModel.WorldPosition;
		}
		CreateInsideOutCube();
	}

	private void OnDestroy()
	{
		if (constraint is ModelingDynamicBoxConstraint modelingDynamicBoxConstraint)
		{
			modelingDynamicBoxConstraint.BoxChanged -= Constraint_BoxChanged;
		}
	}

	private void Constraint_BoxChanged(object sender, ConstraintBoxChangedEventArgs e)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (constraint is ModelingDynamicBoxConstraint)
		{
			((Component)this).transform.position = e.Center;
		}
	}

	private void CreateInsideOutCube()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] vertices = null;
		if (constraint is ModelingDynamicBoxConstraint)
		{
			ModelingDynamicBoxConstraint modelingDynamicBoxConstraint = constraint as ModelingDynamicBoxConstraint;
			((Component)this).transform.localScale = new Vector3((float)modelingDynamicBoxConstraint.Size.x * ((Component)this).transform.localScale.x, (float)modelingDynamicBoxConstraint.Size.y * ((Component)this).transform.localScale.y, (float)modelingDynamicBoxConstraint.Size.z * ((Component)this).transform.localScale.z);
			vertices = SharedCubeFunctions.GetVertices();
		}
		else if (constraint is ModelingBoxConstraint)
		{
			ModelingBoxConstraint modelingBoxConstraint = constraint as ModelingBoxConstraint;
			Vector3[] corners = SharedCubeFunctions.GetCorners(modelingBoxConstraint.FMinCorner - Vector3.one * 0.5f, modelingBoxConstraint.FMaxCorner + Vector3.one * 0.5f);
			vertices = SharedCubeFunctions.GetVertices(corners);
		}
		BuildMesh(vertices);
	}

	private void BuildMesh(Vector3[] vertices)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected Obj, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		MeshFilter val = ((Component)this).gameObject.AddComponent<MeshFilter>();
		MeshRenderer val2 = ((Component)this).gameObject.AddComponent<MeshRenderer>();
		((Renderer)val2).material = (Material)Resources.Load("Materials/ModelConstraints");
		List<int> list = new List<int>();
		List<Vector2> list2 = new List<Vector2>();
		Mesh mesh = val.mesh;
		mesh.vertices = vertices;
		for (int i = 0; i < 6; i++)
		{
			list.Add(i * 4 + 2);
			list.Add(i * 4 + 3);
			list.Add(i * 4);
			list.Add(i * 4);
			list.Add(i * 4 + 1);
			list.Add(i * 4 + 2);
			list2.Add(new Vector2(0f, 0f));
			list2.Add(new Vector2(1f, 0f));
			list2.Add(new Vector2(1f, 1f));
			list2.Add(new Vector2(0f, 1f));
		}
		mesh.uv = list2.ToArray();
		mesh.triangles = list.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
}
