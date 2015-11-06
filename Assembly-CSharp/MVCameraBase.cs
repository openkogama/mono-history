using UnityEngine;

public abstract class MVCameraBase : MonoBehaviour
{
	protected IgnoreInputTypes ignoreInputTypes;

	public float cameraRadius = 0.3f;

	protected bool InputActive => !MVGameControllerBase.IPlayModeUI.InLobbyState && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0;

	public abstract CameraType CameraType { get; }

	public virtual void Awake()
	{
	}

	public void camController_onIgnoreInputTypes(object sender, OnIgnoreInputTypesArgs e)
	{
		ignoreInputTypes = e.inputTypes;
	}

	public virtual void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		targetTransform.position = transform.position;
		targetTransform.rotation = transform.rotation;
	}

	public virtual void Enter(MVCameraController camController)
	{
	}

	public virtual void Exit(MVCameraController camController)
	{
	}

	public virtual void Reset()
	{
	}

	public virtual void FocusOnObject(MVWorldObjectClient wo)
	{
	}
}
