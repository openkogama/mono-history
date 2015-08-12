using UnityEngine;

public class MVGUIAvatarAccessoryMoveIcon : MonoBehaviour
{
	public UXIconButton yTranslateUp;

	public UXIconButton yTranslateDown;

	private UXCamera uxCamera;

	private Camera mainCamera;

	public Vector3 WorldPosition { get; set; }

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
		uxCamera = UXUtils.UXCamera;
		mainCamera = MVGameController.Game.CameraController.GetComponent<Camera>();
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

	public void SetVisible(bool visible)
	{
		yTranslateUp.SetVisible(visible);
		yTranslateDown.SetVisible(visible);
	}
}
