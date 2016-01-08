using MV.WorldObject;
using UnityEngine;

public class CellCursorCubeMesh
{
	private IntVector pos = default;

	private float fadeOutTime = 0.5f;

	private float prevCursorSetTime;

	private GameObject gameObject;

	public float PrevCursorSetTime => prevCursorSetTime;

	public IntVector LocalPos => pos;

	public GameObject GameObject => gameObject;

	public CellCursorCubeMesh()
	{
		gameObject = new GameObject("CellCursor");
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshRenderer.material = PrefabPool.Instance.ModelCubeSpaceMaterial;
		SharedCubeFunctions.AddCubeMesh(meshFilter.mesh, CubeBase.IdentityCorners, insideOut: true);
	}

	public void SetCursorCube(IntVector position, GameObject cubeGameObject)
	{
		pos = position;
		prevCursorSetTime = Time.time;
		Transform transform = gameObject.transform;
		Vector3 position2 = SharedCubeFunctions.LocalToWorld(cubeGameObject, pos);
		gameObject.transform.position = position2;
		transform.position = position2;
		gameObject.transform.localScale = cubeGameObject.transform.localScale * 0.99f;
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
				color.a = num / fadeOutTime;
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
