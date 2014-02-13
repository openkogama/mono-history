using System.Runtime.CompilerServices;
using UnityEngine;

public class MVGUIAvatarAccessoryMoveIcon : MonoBehaviour
{
	public UXIconButton yTranslateUp;

	public UXIconButton yTranslateDown;

	private UXCamera uxCamera;

	private Camera mainCamera;

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

	public void Awake()
	{
		InitializeGizmo();
	}

	public void Start()
	{
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

	public void SetVisible(bool visible)
	{
		yTranslateUp.SetVisible(visible);
		yTranslateDown.SetVisible(visible);
	}
}
