using System;
using System.Collections.Generic;
using UnityEngine;

public class MVCameraController : MonoBehaviour
{
	public TransitionCamera transitionCamera;

	public Transform playModeCamera;

	public Transform jetPackCamera;

	public Shader transparentMultiplyColor;

	public bool freezeCamera;

	private readonly Dictionary<CameraType, MVCameraBase> cameraes = new Dictionary<CameraType, MVCameraBase>();

	private MVCameraBase curCamera;

	private ILogger logger;

	private bool cameraLocked;

	private Transform secondaryCamera;

	private Transform tertiaryCamera;

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

	public CameraType CurrentCameraType => currentCameraType;

	public event EventHandler<OnIgnoreInputTypesArgs> onIgnoreInputTypes;

	public void Init()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		logger = LoggerManager.Instance.GetLogger(typeof(MVCameraController));
		((Component)this).transform.position = MVGameController.Instance.WOCM.LocalPlayer.Avatar.GameObject.transform.position;
		((Component)this).transform.rotation = MVGameController.Instance.WOCM.LocalPlayer.Avatar.GameObject.transform.rotation;
		cameraes[CameraType.ThirdPerson] = ((Component)playModeCamera).gameObject.GetComponent<MVCameraBase>();
		cameraes[CameraType.JetPackCamera] = ((Component)jetPackCamera).gameObject.GetComponent<MVCameraBase>();
		foreach (KeyValuePair<CameraType, MVCameraBase> camerae in cameraes)
		{
			camerae.Value.Init(this);
		}
		transitionCamera.Init(this);
		secondaryCamera = GameObject.Find("Secondary Camera").transform;
		((Component)secondaryCamera).gameObject.active = false;
		if ((Object)(object)secondaryCamera == (Object)null)
		{
			Debug.LogWarning((object)"Secondary camera not found");
		}
		tertiaryCamera = GameObject.Find("Tertiary Camera").transform;
		((Component)tertiaryCamera).gameObject.active = false;
		if ((Object)(object)tertiaryCamera == (Object)null)
		{
			Debug.LogWarning((object)"tertiaryCamera camera not found");
		}
		GameObject.Find("Main Camera").camera.cullingMask = ~((1 << LayerMask.NameToLayer("UXElement")) | (1 << LayerMask.NameToLayer("Preview")));
	}

	public void Respawn()
	{
		Debug.Log((object)("Respawn " + ((object)curCamera).GetType()));
		curCamera.Respawn();
	}

	public void IgnoreInputTypes(IgnoreInputTypes inputTypes)
	{
		if (onIgnoreInputTypes != null)
		{
			onIgnoreInputTypes(this, new OnIgnoreInputTypesArgs(inputTypes));
		}
	}

	public void DrawPlaneMoved(Vector3 to)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<CameraType, MVCameraBase> camerae in cameraes)
		{
			camerae.Value.DrawPlaneMoved(to);
		}
	}

	public void SetCamera(CameraType cameraType)
	{
		Debug.Log((object)("Setting camera to " + cameraType));
		logger.Log(string.Concat("SetCamera(", cameraType, ")."));
		currentCameraType = cameraType;
		if ((Object)(object)curCamera != (Object)null)
		{
			curCamera.Exit(this);
		}
		curCamera = cameraes[cameraType];
		curCamera.Enter(this);
	}

	public void StartTransitionCam(float transitionTime = 2f, bool soft = false)
	{
		transitionCamera.InitTransition(this, ((Component)curCamera).transform, transitionTime, soft);
	}

	public void ForceCamera(Vector3 pos, Vector3 lookAt)
	{
		Debug.LogWarning((object)"Force Camera is not implemented!");
	}

	public void UpdateCamera()
	{
		if (freezeCamera)
		{
			return;
		}
		if (transitionCamera.rotPercentage < 1f)
		{
			if (!cameraLocked)
			{
				curCamera.UpdateCamera(this, ((Component)this).transform);
			}
			transitionCamera.UpdateCamera(this, ((Component)this).transform);
		}
		else if (!cameraLocked)
		{
			curCamera.UpdateCamera(this, ((Component)this).transform);
		}
	}

	public void HandleInput()
	{
		curCamera.HandleInput();
	}

	public void ToggleCameraLock()
	{
		cameraLocked = !cameraLocked;
	}
}
