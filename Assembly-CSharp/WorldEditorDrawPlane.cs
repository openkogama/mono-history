using MV.WorldObject;
using UnityEngine;

public class WorldEditorDrawPlane
{
	public delegate void OnToggleDrawPlaneDelegate(bool state);

	private Plane editorPlane;

	private Vector3 pos;

	private bool isActive = true;

	private GameObject targetGameObject;

	private float lastMovePlaneDelta;

	private DrawPlaneVisualization drawPlaneVisualization;

	private DrawPlaneVisualization drawPlaneCursor;

	public static OnToggleDrawPlaneDelegate OnToggleWorkPlane;

	public bool Active
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
			drawPlaneCursor.Active = value;
			drawPlaneVisualization.Active = value;
			if (OnToggleWorkPlane != null)
			{
				OnToggleWorkPlane(isActive);
			}
		}
	}

	public Vector3 Pos
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return pos;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			pos = value;
			drawPlaneVisualization.SetPos(value);
			editorPlane.SetNormalAndPosition(Vector3.up, value);
		}
	}

	public Quaternion Rot
	{
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			drawPlaneVisualization.SetRot(value);
			drawPlaneCursor.SetRot(value);
		}
	}

	public WorldEditorDrawPlane(GameObject gameObject)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		isActive = true;
		editorPlane = new Plane(Vector3.up, Vector3.zero);
		drawPlaneVisualization = new DrawPlaneVisualization("Prefabs/DrawPlane", 0.001f);
		drawPlaneCursor = new DrawPlaneVisualization("Prefabs/MouseCursorDrawplane", 0.002f);
		SetScale(gameObject.transform.localScale);
		targetGameObject = gameObject;
		Rot = targetGameObject.transform.rotation;
		Pos = SharedCubeFunctions.WorldPosToValidGridPos(gameObject, pos, 1);
	}

	public void Destroy()
	{
		drawPlaneCursor.Destroy();
		drawPlaneVisualization.Destroy();
	}

	public void SetLayer(string layer)
	{
		drawPlaneVisualization.SetLayer(layer);
		drawPlaneCursor.SetLayer(layer);
	}

	public IntVector GetCubePosOnDrawplane(GameObject gameObject)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Vector3 hit = default;
		Pick(ref hit);
		if (IsCamAbove())
		{
			hit.y -= 0.5f * gameObject.transform.localScale.y;
		}
		else
		{
			hit.y += 0.5f * gameObject.transform.localScale.y;
		}
		return SharedCubeFunctions.WorldToLocal(gameObject, hit);
	}

	public void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		MoveDrawPlane();
		Vector3 hit = Vector3.zero;
		if (Pick(ref hit))
		{
			float y = hit.y;
			hit = targetGameObject.transform.InverseTransformPoint(hit);
			MathFunctions.RoundVector(ref hit, 0);
			hit = targetGameObject.transform.TransformPoint(hit);
			hit.y = y;
			drawPlaneCursor.Active = true;
			drawPlaneCursor.SetPos(hit);
		}
		else
		{
			drawPlaneCursor.Active = false;
		}
	}

	public void MoveDrawPlane()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (Time.time - lastMovePlaneDelta > 0.1f)
		{
			if (MVInputWrapper.GetKey((KeyCode)280))
			{
				lastMovePlaneDelta = Time.time;
				num = -1f;
			}
			if (MVInputWrapper.GetKey((KeyCode)281))
			{
				lastMovePlaneDelta = Time.time;
				num = 1f;
			}
		}
		if (num != 0f)
		{
			Vector3 val = Vector3.up * targetGameObject.transform.localScale.y * num;
			Pos -= val;
		}
	}

	public void SetToGridAlignedPos(Vector3 pos)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Pos = SharedCubeFunctions.ToGridAlignedPos(targetGameObject.gameObject, pos);
	}

	public void SetToLowerBound()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 worldCenter = SharedCubeFunctions.GetWorldCenter(upper: false, targetGameObject);
		Rot = targetGameObject.transform.rotation;
		Pos = SharedCubeFunctions.ToGridAlignedPos(targetGameObject, worldCenter);
	}

	public void SetToCenter()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Rot = targetGameObject.transform.rotation;
		Pos = SharedCubeFunctions.ToGridAlignedPos(targetGameObject, Vector3.zero);
	}

	private void SetScale(Vector3 scale)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		drawPlaneVisualization.SetScale(scale);
		drawPlaneCursor.SetScale(scale);
	}

	public bool RayCast(Ray ray, ref Vector3 hit, bool ignoreActiveFlag)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!isActive && !ignoreActiveFlag)
		{
			return false;
		}
		float num = default;
		if (editorPlane.Raycast(ray, ref num))
		{
			Vector3 origin = ray.origin;
			Vector3 direction = ray.direction;
			hit = origin + direction.normalized * num;
			return true;
		}
		return false;
	}

	public bool Pick(ref Vector3 hit)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		return RayCast(ray, ref hit, ignoreActiveFlag: false);
	}

	public bool IsCamAbove()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (pos.y >= ((Component)MVGameController.Instance.WOCM.WeCamera).transform.position.y)
		{
			return true;
		}
		return false;
	}
}
