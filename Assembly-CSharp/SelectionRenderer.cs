using System.Collections.Generic;
using UnityEngine;

public class SelectionRenderer : MonoBehaviour
{
	[SerializeField]
	private Color color = new Color(0f, 1f, 0f, 0.5f);

	[SerializeField]
	private Color wireColor = new Color(0f, 1f, 0f, 1f);

	[SerializeField]
	private Material tintMaterial;

	[SerializeField]
	private Material wireMaterial;

	[SerializeField]
	private List<MeshFilter> meshFilters = new List<MeshFilter>();

	[Range(0.001f, 2f)]
	[SerializeField]
	private float fadeOutTime = 0.5f;

	[Range(0.001f, 2f)]
	[SerializeField]
	private float fadeInTime = 0.5f;

	private float strength;

	private bool fadingIn;

	private void ValidateTint()
	{
		if (tintMaterial == null)
		{
			tintMaterial = (Material)Resources.Load("SelectionTint", typeof(Material));
		}
		if (tintMaterial != null)
		{
			tintMaterial.color = color;
		}
	}

	private void ValidateWire()
	{
		if (wireMaterial == null)
		{
			wireMaterial = (Material)Resources.Load("WireFrame", typeof(Material));
		}
		if (wireMaterial != null)
		{
			wireMaterial.color = wireColor;
		}
	}

	private void OnValidate()
	{
		if (meshFilters.Count == 0)
		{
			meshFilters.AddRange(GetComponents<MeshFilter>());
		}
		ValidateTint();
		ValidateWire();
	}

	private void Awake()
	{
		if (tintMaterial != null)
		{
			tintMaterial = new Material(tintMaterial);
		}
		if (wireMaterial != null)
		{
			wireMaterial = new Material(wireMaterial);
		}
	}

	public void AddMeshFilter(MeshFilter m)
	{
		meshFilters.Add(m);
	}

	public void AddMeshFilters(MeshFilter[] m)
	{
		meshFilters.AddRange(m);
	}

	public void Render()
	{
		strength += Time.deltaTime / fadeInTime;
		if (strength > 1f)
		{
			strength = 1f;
		}
		fadingIn = true;
	}

	private void LateUpdate()
	{
		if (strength > 0f)
		{
			if (!fadingIn)
			{
				FadeOut();
			}
			fadingIn = false;
			tintMaterial.color = color * strength;
			wireMaterial.color = wireColor * strength;
			for (int i = 0; i < meshFilters.Count; i++)
			{
				Draw(meshFilters[i].sharedMesh, meshFilters[i].transform);
			}
		}
	}

	private void FadeOut()
	{
		strength -= Time.deltaTime / fadeOutTime;
		if (strength < 0f)
		{
			strength = 0f;
		}
	}

	private void Draw(Mesh mesh, Transform meshTransform)
	{
		Matrix4x4 localToWorldMatrix = meshTransform.localToWorldMatrix;
		Mesh mesh2 = new Mesh();
		mesh2.vertices = mesh.vertices;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			mesh2.SetIndices(mesh.GetIndices(i), MeshTopology.LineStrip, i);
			Graphics.DrawMesh(mesh2, localToWorldMatrix, wireMaterial, 0);
			Graphics.DrawMesh(mesh, localToWorldMatrix, tintMaterial, 0);
		}
	}
}
