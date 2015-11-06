using MV.WorldObject;
using UnityEngine;
using UnityEngine.Rendering;

public class CellCursorCubeLineMesh
{
	private IntVector pos = default;

	private float fadeOutTime = 0.5f;

	private float baseAlpha;

	private float prevCursorSetTime;

	private GameObject gameObject;

	public float PrevCursorSetTime => prevCursorSetTime;

	public IntVector LocalPos => pos;

	public GameObject GameObject => gameObject;

	public CellCursorCubeLineMesh(float diagonalWidth, string material, float fadeOutTime, Vector3[] cubeCorners)
	{
		this.fadeOutTime = fadeOutTime;
		gameObject = new GameObject("CellCursorCubeLineMesh");
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshRenderer.material = (Material)Resources.Load(material);
		baseAlpha = meshRenderer.material.GetColor("_Color").a;
		SharedCubeFunctions.AddCubeMeshCubeLines(meshFilter.mesh, cubeCorners, diagonalWidth);
	}

	public void SetCursorCube(IntVector position, GameObject cubeGameObject)
	{
		pos = position;
		prevCursorSetTime = Time.time;
		Transform transform = gameObject.transform;
		Vector3 position2 = SharedCubeFunctions.LocalToWorld(cubeGameObject, pos);
		gameObject.transform.position = position2;
		transform.position = position2;
		gameObject.transform.localScale = cubeGameObject.transform.localScale;
		gameObject.transform.rotation = cubeGameObject.transform.rotation;
		gameObject.SetActive(value: true);
	}

	public void Update()
	{
		float num = fadeOutTime - (Time.time - prevCursorSetTime);
		if (num > 0f)
		{
			Material[] materials = gameObject.GetComponent<Renderer>().materials;
			foreach (Material material in materials)
			{
				material.hideFlags = HideFlags.DontSave;
				Color color = material.GetColor("_Color");
				color.a = baseAlpha * (num / fadeOutTime);
				material.SetColor("_Color", color);
			}
		}
		else
		{
			gameObject.SetActive(value: false);
		}
	}

	public void Destroy()
	{
		Object.Destroy(gameObject);
	}
}
