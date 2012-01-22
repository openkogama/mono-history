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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected Obj, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected Obj, but got Unknown
		gameObject = new GameObject("CellCursor");
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer val = gameObject.AddComponent<MeshRenderer>();
		MeshFilter val2 = gameObject.AddComponent<MeshFilter>();
		((Renderer)val).material = (Material)Resources.Load("Materials/ModelCubeSpace");
		SharedCubeFunctions.AddCubeMesh(val2.mesh, CubeBase.IdentityCorners, insideOut: true);
	}

	public void SetCursorCube(IntVector position, GameObject cubeGameObject)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		pos = position;
		prevCursorSetTime = Time.time;
		Transform transform = gameObject.transform;
		Vector3 position2 = SharedCubeFunctions.LocalToWorld(cubeGameObject, pos);
		gameObject.transform.position = position2;
		transform.position = position2;
		gameObject.transform.localScale = cubeGameObject.transform.localScale * 0.99f;
		gameObject.transform.rotation = cubeGameObject.transform.rotation;
		gameObject.active = true;
	}

	public void Update()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		float num = fadeOutTime - (Time.time - prevCursorSetTime);
		if (num > 0f)
		{
			Material[] materials = gameObject.renderer.materials;
			foreach (Material val in materials)
			{
				((Object)val).hideFlags = (HideFlags)4;
				Color color = val.GetColor("_Color");
				color.a = num / fadeOutTime;
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
