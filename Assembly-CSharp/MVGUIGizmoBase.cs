using UnityEngine;

public abstract class MVGUIGizmoBase : MonoBehaviour
{
	public delegate void GizmoClickDelegate();

	private UXCamera uxCamera;

	private Camera mainCamera;

	private bool visible;

	public Vector3 WorldPosition { get; set; }

	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
			UpdateVisibility();
		}
	}

	public void Awake()
	{
		InitializeGizmo();
	}

	public void Start()
	{
		Visible = false;
	}

	public void Update()
	{
		UpdatePosition();
	}

	public void OnEnable()
	{
		UpdatePosition();
	}

	protected virtual void InitializeGizmo()
	{
		uxCamera = UXUtils.UXCamera;
		mainCamera = MVGameControllerBase.CameraController.MainCamera;
	}

	protected void UpdatePosition()
	{
		Vector3 position = mainCamera.WorldToScreenPoint(WorldPosition);
		if (position.z < -1f)
		{
			position.x = -20f;
			position.y = -20f;
		}
		position.z = 10f;
		transform.position = uxCamera.GetComponent<Camera>().ScreenToWorldPoint(position);
	}

	protected abstract void UpdateVisibility();
}
