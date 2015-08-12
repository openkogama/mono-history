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
			_targetGameObject = value;
			transform.parent = _targetGameObject.transform;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;
			transform.localPosition = GetDirectionVector() * 0.5f;
			int layer = (IsOnLandscape ? LayerUtil.GetLayerNumber(LayerFlags.UIItems) : _targetGameObject.layer);
			SetLayer(layer);
			UpdateEditorPlanePosition();
		}
	}

	public bool IsOnLandscape => _targetGameObject == MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().GameObject;

	public Vector3 Pos
	{
		get
		{
			return transform.localPosition;
		}
		private set
		{
			transform.localPosition = value;
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
			DrawPlaneVisualization.SetActive(value);
			DrawPlaneCursor.SetActive(value);
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
			switch (value)
			{
			case DrawPlaneAxis.X:
				transform.localRotation = Quaternion.AngleAxis(90f, Vector3.back);
				break;
			case DrawPlaneAxis.Y:
				transform.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
				break;
			case DrawPlaneAxis.Z:
				transform.localRotation = Quaternion.AngleAxis(90f, Vector3.right);
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
		_cachedPos = Pos;
	}

	public void RestorePos()
	{
		Pos = _cachedPos;
		_cachedPos = Vector3.zero;
	}

	public void Start()
	{
		GenerateDrawPlane(DrawPlaneVisualization);
		DrawPlaneVisualization.GetComponent<Renderer>().material.mainTextureScale = new Vector2(MeshScale, MeshScale);
		DrawPlaneVisualization.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.5f, 0.5f);
		GenerateDrawPlane(DrawPlaneCursor);
	}

	private void GenerateDrawPlane(GameObject drawPlane)
	{
		MeshFilter component = drawPlane.GetComponent<MeshFilter>();
		GenerateMesh(component.mesh, drawPlane == DrawPlaneVisualization);
	}

	private void GenerateMesh(Mesh mesh, bool scale = true)
	{
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
		mesh.name = "DrawPlaneMesh";
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.uv = array3;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	private void SetLayer(int layer)
	{
		DrawPlaneVisualization.layer = layer;
		DrawPlaneCursor.layer = layer;
	}

	public void SetToCameraPos()
	{
		Vector3 forward = Camera.main.transform.forward;
		Vector3 vector = new Vector3((!(forward.x > 0.1f)) ? (-5f) : 5f, (!(forward.y > 0.1f)) ? (-5f) : 5f, (!(forward.z > 0f)) ? (-7f) : 5f);
		SetToGridAlignedPos(MVGameController.WOCM.AvatarLocal.GameObject.transform.position + vector);
	}

	public void SetToGridAlignedPos(Vector3 pos)
	{
		Vector3 vector = _targetGameObject.transform.InverseTransformPoint(pos);
		vector = MathFunctions.RoundVector(vector, 0);
		Pos = vector.Multiply(GetDirectionVector()) + GetOffsetVector();
	}

	public void SetToTargetGameObjectZero()
	{
		SetToGridAlignedPos(_targetGameObject.transform.position);
	}

	private void UpdateEditorPlanePosition()
	{
		editorPlane.SetNormalAndPosition(transform.localToWorldMatrix.MultiplyVector(Vector3.up).normalized, transform.localToWorldMatrix.MultiplyPoint(Vector3.zero));
	}

	public bool GetCubePosOnDrawplane(GameObject gameObject, out IntVector intVectorHitPos)
	{
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
		Altitude = (int)(transform.localPosition.x + transform.localPosition.y + transform.localPosition.z - 0.5f);
	}

	public void UpdateDrawPlane()
	{
		CheckInput();
		Vector3 hit = Vector3.zero;
		if (Pick(ref hit))
		{
			hit = transform.transform.InverseTransformPoint(hit);
			hit = MathFunctions.RoundVector(hit, 0);
			DrawPlaneCursor.SetActive(value: true);
			DrawPlaneCursor.transform.localPosition = hit;
		}
		else
		{
			DrawPlaneCursor.SetActive(value: false);
		}
		FollowAvatar();
	}

	private void FollowAvatar()
	{
		Vector3 vector = transform.InverseTransformPoint(MVGameController.Game.CameraController.transform.position);
		vector = MathFunctions.RoundVector(vector, 0);
		vector.y = 0f;
		DrawPlaneVisualization.transform.localPosition = vector;
	}

	private Vector3 GetDirectionVector()
	{
		return transform.localRotation * Vector3.up;
	}

	public void CheckInput()
	{
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveDrawPlaneDown))
		{
			MoveDrawPlane(-1);
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveDrawPlaneUp))
		{
			MoveDrawPlane(1);
		}
	}

	public void MoveDrawPlane(int dir)
	{
		if (Time.time - lastMovePlaneDelta > 0.1f)
		{
			Pos += transform.localRotation * ((dir != 1) ? Vector3.down : Vector3.up);
			lastMovePlaneDelta = Time.time;
		}
	}

	public bool Pick(ref Vector3 hit)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		return RayCast(ray, ref hit, ignoreActiveFlag: false);
	}

	private bool RayCast(Ray ray, ref Vector3 hit, bool ignoreActiveFlag)
	{
		if (!isActive && !ignoreActiveFlag)
		{
			return false;
		}
		if (editorPlane.Raycast(ray, out var enter))
		{
			hit = ray.origin + ray.direction.normalized * enter;
			return true;
		}
		return false;
	}

	private Vector3 GetCubePlaceOffset()
	{
		Vector3 vector = transform.InverseTransformPoint(MVGameController.Game.CameraController.transform.position);
		Vector3 offsetVector = GetOffsetVector();
		return offsetVector * ((vector.y > 0f) ? 1 : (-1));
	}

	private Vector3 GetOffsetVector()
	{
		return 0.5f * GetDirectionVector();
	}
}
