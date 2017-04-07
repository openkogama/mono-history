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

	[SerializeField]
	[Range(0.001f, 2f)]
	private float fadeOutTime = 0.5f;

	[Range(0.001f, 2f)]
	[SerializeField]
	private float fadeInTime = 0.5f;

	private List<Mesh> wireMeshes;

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
		wireMeshes = new List<Mesh>(meshFilters.Count);
		for (int i = 0; i < meshFilters.Count; i++)
		{
			wireMeshes.Add(new Mesh());
		}
	}

	public void AddMeshFilter(MeshFilter m)
	{
		meshFilters.Add(m);
		wireMeshes.Add(new Mesh());
	}

	public void AddMeshFilters(MeshFilter[] m)
	{
		meshFilters.AddRange(m);
		for (int i = 0; i < m.Length; i++)
		{
			wireMeshes.Add(new Mesh());
		}
	}

	public void Show()
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
		if (!(strength > 0f))
		{
			return;
		}
		if (!fadingIn)
		{
			FadeOut();
		}
		fadingIn = false;
		tintMaterial.color = color * strength;
		wireMaterial.color = wireColor * strength;
		for (int i = 0; i < meshFilters.Count; i++)
		{
			Matrix4x4 localToWorldMatrix = meshFilters[i].transform.localToWorldMatrix;
			Mesh mesh = wireMeshes[i];
			mesh.vertices = meshFilters[i].sharedMesh.vertices;
			for (int j = 0; j < meshFilters[i].sharedMesh.subMeshCount; j++)
			{
				mesh.SetIndices(meshFilters[i].sharedMesh.GetIndices(j), MeshTopology.LineStrip, j);
			}
			Graphics.DrawMesh(mesh, localToWorldMatrix, wireMaterial, 0);
			Graphics.DrawMesh(meshFilters[i].sharedMesh, localToWorldMatrix, tintMaterial, 0);
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
}
