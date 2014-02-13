using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVCameraController : MonoBehaviour
{
	public TransitionCamera transitionCamera;

	public Transform playModeCamera;

	public Transform jetPackCamera;

	public Transform freeRoamCamera;

	public Transform avatarAccessoryCamera;

	public Transform orbitCamera;

	public Shader transparentMultiplyColor;

	private readonly Dictionary<CameraType, MVCameraBase> cameras = new Dictionary<CameraType, MVCameraBase>();

	private MVCameraBase curCamera;

	private ILogger logger;

	public Transform secondaryCamera;

	public Transform tertiaryCamera;

	private CameraType currentCameraType;

	public bool IgnoreInput { get; set; }

	public bool SecondaryCameraActive
	{
		get
		{
			return ((Component)secondaryCamera).gameObject.active;
		}
		set
		{
			((Component)secondaryCamera).gameObject.active = value;
		}
	}

	public bool TertiaryCameraActive
	{
		get
		{
			return ((Component)tertiaryCamera).gameObject.active;
		}
		set
		{
			((Component)tertiaryCamera).gameObject.active = value;
		}
	}

	public Camera TertiaryCamera => ((Component)tertiaryCamera).GetComponent<Camera>();

	public MVCameraBase CurCamera => curCamera;

	public event EventHandler<OnIgnoreInputTypesArgs> onIgnoreInputTypes;

	public T GetCamera<T>() where T : MVCameraBase
	{
		foreach (KeyValuePair<CameraType, MVCameraBase> camera in cameras)
		{
			if ((object)((object)camera.Value).GetType() == typeof(T))
			{
				return (T)camera.Value;
			}
		}
		return (T)null;
	}

	private void Awake()
	{
		logger = LoggerManager.Instance.GetLogger(typeof(MVCameraController));
		cameras[CameraType.ThirdPerson] = ((Component)playModeCamera).gameObject.GetComponent<MVCameraBase>();
		cameras[CameraType.JetPackCamera] = ((Component)jetPackCamera).gameObject.GetComponent<MVCameraBase>();
		cameras[CameraType.FreeRoam] = ((Component)freeRoamCamera).gameObject.GetComponent<MVCameraBase>();
		cameras[CameraType.AvatarAccessory] = ((Component)avatarAccessoryCamera).gameObject.GetComponent<MVCameraBase>();
		cameras[CameraType.OrbitCamera] = ((Component)orbitCamera).gameObject.GetComponent<MVCameraBase>();
		foreach (KeyValuePair<CameraType, MVCameraBase> camera in cameras)
		{
			camera.Value.Init(this);
		}
		MVGameController.Instance.Game.CameraController = this;
		transitionCamera.Init(this);
	}

	public void Init()
	{
		((Component)this).gameObject.camera.cullingMask = ~((1 << LayerMask.NameToLayer("UXElement")) | (1 << LayerMask.NameToLayer("Preview")) | (1 << LayerMask.NameToLayer("Hidden")));
		Camera camera = ((Component)MVGameController.Instance.Game.CameraController).camera;
		if (MVGameController.Instance.Game.GameMode == MVGameMode.Play && (camera.cullingMask & LayerMask.NameToLayer("Logic")) == LayerMask.NameToLayer("Logic"))
		{
			Camera camera2 = ((Component)this).gameObject.camera;
			camera2.cullingMask -= 1 << (LayerMask.NameToLayer("Logic") & 0x1F);
		}
	}

	public bool RequestCursorLock()
	{
		MVGUIAskForFocus mVGUIAskForFocus = UXUtils.FindGUIObjectOfType<MVGUIAskForFocus>();
		MVGUIMenu mVGUIMenu = UXUtils.FindGUIObjectOfType<MVGUIMenu>();
		UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		Screen.lockCursor = !mVGUIMenu.View.isVisible && !uXDialogFactory.DialogOpen;
		if (Screen.lockCursor)
		{
			mVGUIAskForFocus.RegainFocus();
		}
		return Screen.lockCursor;
	}

	public void Respawn()
	{
		curCamera.Respawn();
	}

	public void IgnoreInputTypes(IgnoreInputTypes inputTypes)
	{
		if (onIgnoreInputTypes != null)
		{
			onIgnoreInputTypes(this, new OnIgnoreInputTypesArgs(inputTypes));
		}
	}

	public void SetCamera(CameraType cameraType)
	{
		logger.Log(string.Concat("SetCamera(", cameraType, ")."));
		EnterCamera(cameras[cameraType]);
	}

	public void SetCamera(MVCameraBase newCamera)
	{
		newCamera.Init(this);
		EnterCamera(newCamera);
	}

	private void EnterCamera(MVCameraBase newCamera)
	{
		if ((Object)(object)curCamera != (Object)null)
		{
			curCamera.Exit(this);
		}
		curCamera = newCamera;
		curCamera.Enter(this);
	}

	public void StartTransitionCam(float transitionTime = 2f, bool soft = false)
	{
		transitionCamera.InitTransition(this, ((Component)curCamera).transform, transitionTime, soft);
	}

	public void UpdateCamera()
	{
		if (transitionCamera.rotPercentage < 1f)
		{
			curCamera.UpdateCamera(this, ((Component)this).transform);
			transitionCamera.UpdateCamera(this, ((Component)this).transform);
		}
		else
		{
			curCamera.UpdateCamera(this, ((Component)this).transform);
		}
	}

	public void HandleInput()
	{
		curCamera.HandleInput(this);
	}
}
