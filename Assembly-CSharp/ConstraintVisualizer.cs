using System.Collections.Generic;
using UnityEngine;

public class ConstraintVisualizer : MonoBehaviour
{
	private IModelingConstraint constraint;

	public void Init(MVCubeModelBase targetCubeModel, IModelingConstraint constraint, string layer = "UIItems")
	{
		this.constraint = constraint;
		gameObject.layer = LayerMask.NameToLayer(layer);
		transform.rotation = targetCubeModel.WorldRotation;
		transform.localScale = targetCubeModel.Scale;
		transform.parent = targetCubeModel.GameObject.transform;
		if (constraint is ModelingDynamicBoxConstraint)
		{
			ModelingDynamicBoxConstraint modelingDynamicBoxConstraint = (ModelingDynamicBoxConstraint)constraint;
			transform.position = modelingDynamicBoxConstraint.Center;
			modelingDynamicBoxConstraint.BoxChanged += Constraint_BoxChanged;
		}
		else
		{
			transform.position = targetCubeModel.WorldPosition;
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
		if (constraint is ModelingDynamicBoxConstraint)
		{
			transform.position = e.Center;
		}
	}

	private void CreateInsideOutCube()
	{
		Vector3[] vertices = null;
		if (constraint is ModelingDynamicBoxConstraint)
		{
			ModelingDynamicBoxConstraint modelingDynamicBoxConstraint = constraint as ModelingDynamicBoxConstraint;
			transform.localScale = new Vector3((float)(short)modelingDynamicBoxConstraint.Size.x * transform.localScale.x, (float)(short)modelingDynamicBoxConstraint.Size.y * transform.localScale.y, (float)(short)modelingDynamicBoxConstraint.Size.z * transform.localScale.z);
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
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = (Material)Resources.Load("Materials/ModelConstraints");
		List<int> list = new List<int>();
		List<Vector2> list2 = new List<Vector2>();
		Mesh mesh = meshFilter.mesh;
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
