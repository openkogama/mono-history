using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MainCameraManager : MonoBehaviour
{
	[SerializeField]
	private Camera mainCamera;

	[SerializeField]
	private Transform secondaryCamera;

	[SerializeField]
	private Transform tertiaryCamera;

	[SerializeField]
	private TransitionCamera transitionCamera;

	[SerializeField]
	private AudioSource plingSound;

	[SerializeField]
	private GrayscaleEffect greyScaleEffect;

	[SerializeField]
	private Skybox skybox;

	[SerializeField]
	private LineDrawManager lineDrawManager;

	private MVCameraController cameraController;

	private bool isLogicRendered;

	private static Dictionary<MVGameType, ICameraSettings> cameraSettings = new Dictionary<MVGameType, ICameraSettings>();

	public Shader transparentMultiplyColor;

	private bool blueModeEnabled;

	private ProtectedTransform protectedTransform;

	private static float baseVolume = 0f;

	private static bool mute = false;

	public static Action<bool> OnMuteChange;

	public static Action OnCameraSettingAdded;

	private int cullingMask;

	private MaskMode maskMode;

	public Skybox Skybox => skybox;

	public LineDrawManager LineDrawManager => lineDrawManager;

	public float FieldOfView
	{
		get
		{
			return mainCamera.fieldOfView;
		}
		set
		{
			mainCamera.fieldOfView = value;
		}
	}

	public Camera MainCamera => mainCamera;

	public MVCameraBase CurrentCamera => cameraController.CurCamera;

	public bool BlueModeEnabled
	{
		get
		{
			return blueModeEnabled;
		}
		set
		{
			blueModeEnabled = value;
			secondaryCamera.gameObject.SetActive(value);
			greyScaleEffect.enabled = value;
		}
	}

	public ProtectedTransform ProtectedTransform => protectedTransform;

	public Vector3 FireDirection => transform.forward;

	public Vector3 FireOrigin => transform.position + transform.forward * CurrentCamera.cameraRadius;

	public static bool Mute
	{
		get
		{
			return mute;
		}
		set
		{
			mute = value;
			if (mute)
			{
				AudioListener.volume = 0f;
			}
			else
			{
				AudioListener.volume = baseVolume;
			}
			if (OnMuteChange != null)
			{
				OnMuteChange(mute);
			}
		}
	}

	public MaskMode CamMaskMode
	{
		get
		{
			return maskMode;
		}
		set
		{
			maskMode = value;
			switch (maskMode)
			{
			case MaskMode.Default:
				mainCamera.cullingMask = cullingMask;
				blueModeEnabled = false;
				break;
			case MaskMode.AvatarLobbyFocus:
				mainCamera.cullingMask = 1 << LayerMask.NameToLayer("CamRotateTarget");
				blueModeEnabled = true;
				break;
			case MaskMode.SkyBoxOnly:
				mainCamera.cullingMask = 0;
				break;
			}
		}
	}

	public bool TertiaryCameraActive
	{
		get
		{
			return tertiaryCamera.gameObject.activeInHierarchy;
		}
		set
		{
			tertiaryCamera.gameObject.SetActive(value);
		}
	}

	public Camera TertiaryCamera => tertiaryCamera.GetComponent<Camera>();

	public Camera SecondaryCamera => secondaryCamera.GetComponent<Camera>();

	public bool IsLogicRendered
	{
		get
		{
			return isLogicRendered;
		}
		set
		{
			RenderLogic(value);
		}
	}

	public event EventHandler<OnIgnoreInputTypesArgs> onIgnoreInputTypes;

	public static void RegisterCameraWithSettings(MVGameType gameType, ICameraSettings cameraSettings)
	{
		MainCameraManager.cameraSettings.Add(gameType, cameraSettings);
		if (OnCameraSettingAdded != null)
		{
			OnCameraSettingAdded();
		}
	}

	public static ICameraSettings GetSettings(MVGameType gameType)
	{
		return cameraSettings[gameType];
	}

	public static bool HasSetting(MVGameType gameType)
	{
		return cameraSettings.ContainsKey(gameType);
	}

	public void PlayPlingSound()
	{
		plingSound.Play();
	}

	public void SetCameraController(MVCameraController cameraController)
	{
		this.cameraController = cameraController;
	}

	public bool IsCameraControllerSet()
	{
		return cameraController != null;
	}

	protected void Awake()
	{
		baseVolume = AudioListener.volume;
		Mute = false;
		protectedTransform = new ProtectedTransform(transform);
	}

	public void IgnoreInputTypes(IgnoreInputTypes inputTypes)
	{
		if (onIgnoreInputTypes != null)
		{
			onIgnoreInputTypes(this, new OnIgnoreInputTypesArgs(inputTypes));
		}
	}

	public void Init()
	{
		MainCamera.cullingMask = ~((1 << LayerMask.NameToLayer("UXElement")) | (1 << LayerMask.NameToLayer("Preview")) | (1 << LayerMask.NameToLayer("Hidden")) | (1 << LayerMask.NameToLayer("UXElementSecondary")));
		if (MVGameControllerBase.GameMode == MVGameMode.Play && (MainCamera.cullingMask & LayerMask.NameToLayer("Logic")) == LayerMask.NameToLayer("Logic"))
		{
			MainCamera.cullingMask -= 1 << LayerMask.NameToLayer("Logic");
		}
		cullingMask = MainCamera.cullingMask;
	}

	public void UpdateCamera()
	{
		cameraController.UpdateCamera(protectedTransform);
		transitionCamera.UpdateCamera(cameraController, protectedTransform);
	}

	public void StartTransitionCam(float transitionTime = 2f, bool soft = false)
	{
		transitionCamera.InitTransition(CurrentCamera.transform, transitionTime, soft);
	}

	public void CancelTransitionCam()
	{
		transitionCamera.AbortTransition();
	}

	private void RenderLogic(bool renderLogic)
	{
		if (renderLogic)
		{
			MainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Logic");
		}
		else
		{
			MainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Logic"));
		}
		isLogicRendered = renderLogic;
	}

	protected void OnDestroy()
	{
		cameraSettings.Clear();
	}
}
