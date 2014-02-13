using MV.WorldObject;
using UnityEngine;

public class WorldEditorDrawPlane : MonoBehaviour
{
	public delegate void AltitudeChangedDelegate(int altitude);

	private const float CUBE_OFFSET = 0.5f;

	public AltitudeChangedDelegate OnAltitudeChanged;

	public GameObject DrawPlaneVisualization;

	public GameObject DrawPlaneCursor;

	public int MeshScale = 100;

	private GameObject _targetGameObject;

	private bool isActive = true;

	private int _altitude;

	private float lastMovePlaneDelta;

	private Plane editorPlane;

	private Vector3 _cachedPos;

	public GameObject TargetGameObject
	{
		set
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			_targetGameObject = value;
			((Component)this).transform.parent = _targetGameObject.transform;
			((Component)this).transform.localScale = Vector3.one;
			((Component)this).transform.localRotation = Quaternion.identity;
			((Component)this).transform.localPosition = GetDirectionVector() * 0.5f;
			int layer = (IsOnLandscape ? LayerUtil.GetLayerNumber(LayerFlags.UIItems) : _targetGameObject.layer);
			SetLayer(layer);
			UpdateEditorPlanePosition();
		}
	}

	public bool IsOnLandscape => (Object)(object)_targetGameObject == (Object)(object)MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().GameObject;

	public Vector3 Pos
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.localPosition;
		}
		private set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Component)this).transform.localPosition = value;
			UpdateEditorPlanePosition();
			UpdateAltitude();
		}
	}

	public bool Active
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
			DrawPlaneVisualization.active = value;
			DrawPlaneCursor.active = value;
			UpdateAltitude();
		}
	}

	public int Altitude
	{
		get
		{
			return _altitude;
		}
		private set
		{
			_altitude = value;
			if (OnAltitudeChanged != null)
			{
				OnAltitudeChanged(_altitude);
			}
		}
	}

	public DrawPlaneAxis Orientation
	{
		set
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			switch (value)
			{
			case DrawPlaneAxis.X:
				((Component)this).transform.localRotation = Quaternion.AngleAxis(90f, Vector3.back);
				break;
			case DrawPlaneAxis.Y:
				((Component)this).transform.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
				break;
			case DrawPlaneAxis.Z:
				((Component)this).transform.localRotation = Quaternion.AngleAxis(90f, Vector3.right);
				break;
			}
			if (IsOnLandscape)
			{
				SetToCameraPos();
			}
			else
			{
				SetToTargetGameObjectZero();
			}
			UpdateEditorPlanePosition();
			UpdateAltitude();
		}
	}

	public void CachePos()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_cachedPos = Pos;
	}

	public void RestorePos()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Pos = _cachedPos;
		_cachedPos = Vector3.zero;
	}

	public void Start()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		GenerateDrawPlane(DrawPlaneVisualization);
		DrawPlaneVisualization.renderer.material.mainTextureScale = new Vector2((float)MeshScale, (float)MeshScale);
		DrawPlaneVisualization.renderer.material.mainTextureOffset = new Vector2(0.5f, 0.5f);
		GenerateDrawPlane(DrawPlaneCursor);
	}

	private void GenerateDrawPlane(GameObject drawPlane)
	{
		MeshFilter component = drawPlane.GetComponent<MeshFilter>();
		component.mesh = GenerateMesh((Object)(object)drawPlane == (Object)(object)DrawPlaneVisualization);
	}

	private Mesh GenerateMesh(bool scale = true)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected Obj, but got Unknown
		float num = ((!scale) ? 1f : ((float)MeshScale));
		Vector3[] array = new Vector3[4];
		int[] array2 = new int[6];
		Vector2[] array3 = new Vector2[array.Length];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0.5f, 0f, -0.5f) * num;
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(0.5f, 0f, 0.5f) * num;
		ref Vector3 reference3 = ref array[2];
		reference3 = new Vector3(-0.5f, 0f, 0.5f) * num;
		ref Vector3 reference4 = ref array[3];
		reference4 = new Vector3(-0.5f, 0f, -0.5f) * num;
		array2[0] = 0;
		array2[1] = 1;
		array2[2] = 2;
		array2[3] = 2;
		array2[4] = 3;
		array2[5] = 0;
		ref Vector2 reference5 = ref array3[0];
		reference5 = new Vector2(0f, 0f);
		ref Vector2 reference6 = ref array3[1];
		reference6 = new Vector2(0f, 1f);
		ref Vector2 reference7 = ref array3[2];
		reference7 = new Vector2(1f, 1f);
		ref Vector2 reference8 = ref array3[3];
		reference8 = new Vector2(1f, 0f);
		Mesh val = new Mesh();
		((Object)val).name = "DrawPlaneMesh";
		val.vertices = array;
		val.triangles = array2;
		val.uv = array3;
		val.RecalculateBounds();
		val.RecalculateNormals();
		return val;
	}

	private void SetLayer(int layer)
	{
		DrawPlaneVisualization.layer = layer;
		DrawPlaneCursor.layer = layer;
	}

	public void SetToCameraPos()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Vector3 forward = ((Component)Camera.main).transform.forward;
		Vector3 val = new Vector3((!(forward.x > 0.1f)) ? (-5f) : 5f, (!(forward.y > 0.1f)) ? (-5f) : 5f, (!(forward.z > 0f)) ? (-7f) : 5f);
		SetToGridAlignedPos(MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position + val);
	}

	public void SetToGridAlignedPos(Vector3 pos)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vector = _targetGameObject.transform.InverseTransformPoint(pos);
		vector = MathFunctions.RoundVector(vector, 0);
		Pos = vector.Multiply(GetDirectionVector()) + GetOffsetVector();
	}

	public void SetToTargetGameObjectZero()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		SetToGridAlignedPos(_targetGameObject.transform.position);
	}

	private void UpdateEditorPlanePosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		ref Plane reference = ref editorPlane;
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		Vector3 val = localToWorldMatrix.MultiplyVector(Vector3.up);
		Vector3 normalized = val.normalized;
		Matrix4x4 localToWorldMatrix2 = ((Component)this).transform.localToWorldMatrix;
		reference.SetNormalAndPosition(normalized, localToWorldMatrix2.MultiplyPoint(Vector3.zero));
	}

	public bool GetCubePosOnDrawplane(GameObject gameObject, out IntVector intVectorHitPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Vector3 hit = default;
		bool result = Pick(ref hit);
		hit = _targetGameObject.transform.InverseTransformPoint(hit);
		hit += GetCubePlaceOffset();
		hit = MathFunctions.RoundVector(hit, 0);
		intVectorHitPos = new IntVector((short)hit.x, (short)hit.y, (short)hit.z);
		return result;
	}

	private void UpdateAltitude()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Altitude = (int)(((Component)this).transform.localPosition.x + ((Component)this).transform.localPosition.y + ((Component)this).transform.localPosition.z - 0.5f);
	}

	public void UpdateDrawPlane()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		CheckInput();
		Vector3 hit = Vector3.zero;
		if (Pick(ref hit))
		{
			hit = ((Component)((Component)this).transform).transform.InverseTransformPoint(hit);
			hit = MathFunctions.RoundVector(hit, 0);
			DrawPlaneCursor.active = true;
			DrawPlaneCursor.transform.localPosition = hit;
		}
		else
		{
			DrawPlaneCursor.active = false;
		}
		FollowAvatar();
	}

	private void FollowAvatar()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vector = ((Component)this).transform.InverseTransformPoint(((Component)MVGameController.Instance.Game.CameraController).transform.position);
		vector = MathFunctions.RoundVector(vector, 0);
		vector.y = 0f;
		DrawPlaneVisualization.transform.localPosition = vector;
	}

	private Vector3 GetDirectionVector()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.localRotation * Vector3.up;
	}

	public void CheckInput()
	{
		if (MVInputWrapper.GetKey((KeyCode)280))
		{
			MoveDrawPlane(-1);
		}
		if (MVInputWrapper.GetKey((KeyCode)281))
		{
			MoveDrawPlane(1);
		}
	}

	public void MoveDrawPlane(int dir)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time - lastMovePlaneDelta > 0.1f)
		{
			Pos += ((Component)this).transform.localRotation * ((dir != 1) ? Vector3.up : Vector3.down);
			lastMovePlaneDelta = Time.time;
		}
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

	private bool RayCast(Ray ray, ref Vector3 hit, bool ignoreActiveFlag)
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

	private Vector3 GetCubePlaceOffset()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.InverseTransformPoint(((Component)MVGameController.Instance.Game.CameraController).transform.position);
		Vector3 offsetVector = GetOffsetVector();
		return offsetVector * (float)((val.y > 0f) ? 1 : (-1));
	}

	private Vector3 GetOffsetVector()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return 0.5f * GetDirectionVector();
	}
}
