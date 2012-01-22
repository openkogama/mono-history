using MV.WorldObject;
using UnityEngine;

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

	public CellCursorCubeLineMesh(float diagonalWidth, string material, float fadeOutTime)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected Obj, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected Obj, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		this.fadeOutTime = fadeOutTime;
		gameObject = new GameObject("CellCursorCubeLineMesh");
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer val = gameObject.AddComponent<MeshRenderer>();
		((Renderer)val).castShadows = false;
		((Renderer)val).receiveShadows = false;
		MeshFilter val2 = gameObject.AddComponent<MeshFilter>();
		((Renderer)val).material = (Material)Resources.Load(material);
		baseAlpha = ((Renderer)val).material.GetColor("_Color").a;
		SharedCubeFunctions.AddCubeMeshCubeLines(val2.mesh, CubeBase.IdentityCorners, diagonalWidth);
	}

	public void SetCursorCube(IntVector position, GameObject cubeGameObject)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		pos = position;
		prevCursorSetTime = Time.time;
		Transform transform = gameObject.transform;
		Vector3 position2 = SharedCubeFunctions.LocalToWorld(cubeGameObject, pos);
		gameObject.transform.position = position2;
		transform.position = position2;
		gameObject.transform.localScale = cubeGameObject.transform.localScale;
		gameObject.transform.rotation = cubeGameObject.transform.rotation;
		gameObject.active = true;
	}

	public void Update()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		float num = fadeOutTime - (Time.time - prevCursorSetTime);
		if (num > 0f)
		{
			Material[] materials = gameObject.renderer.materials;
			foreach (Material val in materials)
			{
				((Object)val).hideFlags = (HideFlags)4;
				Color color = val.GetColor("_Color");
				color.a = baseAlpha * (num / fadeOutTime);
				val.SetColor("_Color", color);
			}
		}
		else
		{
			gameObject.active = false;
		}
	}

	public void Destroy()
	{
		Object.Destroy((Object)(object)gameObject);
	}
}
