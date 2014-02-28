using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class MVGUIGizmoBase : MonoBehaviour
{
	public delegate void GizmoClickDelegate();

	private UXCamera uxCamera;

	private Camera mainCamera;

	private bool visible;

	public Vector3 WorldPosition
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

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
		uxCamera = Object.FindObjectOfType(typeof(UXCamera)) as UXCamera;
		mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}

	protected void UpdatePosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = mainCamera.WorldToScreenPoint(WorldPosition);
		if (val.z < -1f)
		{
			val.x = -20f;
			val.y = -20f;
		}
		val.z = 10f;
		((Component)this).transform.position = ((Component)uxCamera).camera.ScreenToWorldPoint(val);
	}

	protected abstract void UpdateVisibility();
}
