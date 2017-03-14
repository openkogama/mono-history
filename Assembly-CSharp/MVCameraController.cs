using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVCameraController : MonoBehaviour
{
	private class CameraStack
	{
		private readonly List<MVCameraBase> activeCameras = new List<MVCameraBase>();

		private readonly Dictionary<CameraType, MVCameraBase> cameras = new Dictionary<CameraType, MVCameraBase>();

		private TransitionCamera TransitionCamera => (TransitionCamera)cameras[CameraType.TransitionCamera];

		public MVCameraBase CurCamera
		{
			get
			{
				if (activeCameras.Count == 0)
				{
					return null;
				}
				return activeCameras[activeCameras.Count - 1];
			}
		}

		public CameraStack(List<MVCameraBase> camerasList, MVCameraController cameraController)
		{
			foreach (MVCameraBase cameras in camerasList)
			{
				this.cameras.Add(cameras.CameraType, cameras);
			}
		}

		public void SetCamera(CameraType cameraType, MVCameraController cameraController)
		{
			EnterCamera(cameras[cameraType], cameraController);
		}

		public void SetCamera(MVCameraBase newCamera, MVCameraController cameraController)
		{
			EnterCamera(newCamera, cameraController);
		}

		public void StartTransitionCam(MVCameraController cameraController, float transitionTime = 2f, bool soft = false)
		{
			TransitionCamera.InitTransition(cameraController, CurCamera.transform, transitionTime, soft);
		}

		public void CancelTransitionCam()
		{
			TransitionCamera.AbortTransition();
		}

		public void UpdateCamera(MVCameraController cameraController)
		{
			CurCamera.UpdateCamera(cameraController, cameraController.transform);
			if (TransitionInProgress())
			{
				TransitionCamera.UpdateCamera(cameraController, cameraController.transform);
				cameraController.FieldOfView = Mathf.Lerp(TransitionCamera.FieldOfView, CurCamera.FieldOfView, TransitionCamera.RotPercentage);
			}
			else
			{
				cameraController.FieldOfView = CurCamera.FieldOfView;
			}
		}

		public bool TransitionInProgress()
		{
			return TransitionCamera.RotPercentage < 1f;
		}

		private void EnterCamera(MVCameraBase newCamera, MVCameraController cameraController)
		{
			ClearStack(cameraController);
			activeCameras.Add(newCamera);
			cameraController.onIgnoreInputTypes = (EventHandler<OnIgnoreInputTypesArgs>)Delegate.Combine(cameraController.onIgnoreInputTypes, new EventHandler<OnIgnoreInputTypesArgs>(CurCamera.camController_onIgnoreInputTypes));
			CurCamera.Enter(cameraController);
		}

		private void ClearStack(MVCameraController cameraController)
		{
			int num = activeCameras.Count - 1;
			for (int num2 = num; num2 >= 0; num2--)
			{
				activeCameras[num2].Exit(cameraController);
				cameraController.onIgnoreInputTypes = (EventHandler<OnIgnoreInputTypesArgs>)Delegate.Remove(cameraController.onIgnoreInputTypes, new EventHandler<OnIgnoreInputTypesArgs>(activeCameras[num2].camController_onIgnoreInputTypes));
				activeCameras.RemoveAt(num2);
			}
		}

		public T GetCamera<T>() where T : MVCameraBase
		{
			foreach (KeyValuePair<CameraType, MVCameraBase> camera in cameras)
			{
				if (camera.Value.GetType() == typeof(T))
				{
					return (T)camera.Value;
				}
			}
			return (T)null;
		}

		public void PushCamera(CameraType cameraType, MVCameraController cameraController)
		{
			PushCamera(cameras[cameraType], cameraController);
		}

		public void PushCamera(MVCameraBase cameraBase, MVCameraController cameraController)
		{
			int count = activeCameras.Count;
			if (count > 0)
			{
				activeCameras[count - 1].Suspend(cameraController);
			}
			activeCameras.Add(cameraBase);
			cameraController.onIgnoreInputTypes = (EventHandler<OnIgnoreInputTypesArgs>)Delegate.Combine(cameraController.onIgnoreInputTypes, new EventHandler<OnIgnoreInputTypesArgs>(CurCamera.camController_onIgnoreInputTypes));
			CurCamera.Enter(cameraController);
		}

		public void RemoveCamera(CameraType cameraType, MVCameraController cameraController)
		{
			RemoveCamera(cameras[cameraType], cameraController);
		}

		public void RemoveCamera(MVCameraBase cameraBase, MVCameraController cameraController)
		{
			int num = activeCameras.Count - 1;
			for (int num2 = num; num2 >= 0; num2--)
			{
				if (activeCameras[num2] == cameraBase)
				{
					activeCameras[num2].Exit(cameraController);
					cameraController.onIgnoreInputTypes = (EventHandler<OnIgnoreInputTypesArgs>)Delegate.Remove(cameraController.onIgnoreInputTypes, new EventHandler<OnIgnoreInputTypesArgs>(CurCamera.camController_onIgnoreInputTypes));
					activeCameras.RemoveAt(num2);
					if (num2 == num && num2 > 0)
					{
						activeCameras[num2 - 1].Resume(cameraController);
					}
					break;
				}
			}
		}
	}

	[SerializeField]
	private GrayscaleEffect greyScaleEffect;

	private CameraStack cameraStack;

	[SerializeField]
	private List<MVCameraBase> cameraBases = new List<MVCameraBase>();

	[SerializeField]
	private Camera mainCamera;

	[SerializeField]
	private Transform secondaryCamera;

	[SerializeField]
	private Transform tertiaryCamera;

	[SerializeField]
	private AvatarCameraFade avatarCameraFade;

	[SerializeField]
	private AudioSource plingSound;

	private bool isLogicRendered;

	private static Dictionary<MVGameType, ICameraSettings> cameraSettings = new Dictionary<MVGameType, ICameraSettings>();

	public Shader transparentMultiplyColor;

	[SerializeField]
	private LineDrawManager lineDrawManager;

	private bool blueModeEnabled;

	private static float baseVolume = 0f;

	private static bool mute = false;

	public static Action<bool> OnMuteChange;

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

	public AvatarCameraFade AvatarCameraFade => avatarCameraFade;

	public Camera MainCamera => mainCamera;

	public LineDrawManager LineDrawManager => lineDrawManager;

	public Vector3 FireDirection => transform.forward;

	public Vector3 FireOrigin => transform.position + transform.forward * CurCamera.cameraRadius;

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

	public MVCameraBase CurCamera => cameraStack.CurCamera;

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

	public event EventHandler<OnIgnoreInputTypesArgs> onIgnoreInputTypes;

	public void PlayPlingSound()
	{
		plingSound.Play();
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

	public T GetCamera<T>() where T : MVCameraBase
	{
		return cameraStack.GetCamera<T>();
	}

	private void Awake()
	{
		cameraStack = new CameraStack(cameraBases, this);
		baseVolume = AudioListener.volume;
		Mute = false;
	}

	public void Init()
	{
		MainCamera.cullingMask = ~((1 << LayerMask.NameToLayer("UXElement")) | (1 << LayerMask.NameToLayer("Preview")) | (1 << LayerMask.NameToLayer("Hidden")) | (1 << LayerMask.NameToLayer("UXElementSecondary")));
		if (MVGameControllerBase.GameMode == MVGameMode.Play && (MainCamera.cullingMask & LayerMask.NameToLayer("Logic")) == LayerMask.NameToLayer("Logic"))
		{
			MainCamera.cullingMask -= 1 << LayerMask.NameToLayer("Logic");
		}
	}

	public static void RegisterCameraWithSettings(MVGameType gameType, ICameraSettings cameraSettings)
	{
		MVCameraController.cameraSettings.Add(gameType, cameraSettings);
	}

	public static ICameraSettings GetSettings(MVGameType gameType)
	{
		return cameraSettings[gameType];
	}

	public void Respawn()
	{
		cameraStack.CurCamera.Reset();
	}

	public void IgnoreInputTypes(IgnoreInputTypes inputTypes)
	{
		if (onIgnoreInputTypes != null)
		{
			onIgnoreInputTypes(this, new OnIgnoreInputTypesArgs(inputTypes));
		}
	}

	public void SetPlayModeCam()
	{
		switch (MVGameControllerBase.Game.GameType)
		{
		case MVGameType.Classic:
			cameraStack.SetCamera(CameraType.ThirdPerson, this);
			break;
		case MVGameType.Platformer:
			cameraStack.SetCamera(CameraType.Platformer, this);
			break;
		}
	}

	public void SetCamera(CameraType cameraType)
	{
		cameraStack.SetCamera(cameraType, this);
	}

	public void SetCamera(MVCameraBase cameraBase)
	{
		cameraStack.SetCamera(cameraBase, this);
	}

	public void PushCamera(CameraType cameraType)
	{
		cameraStack.PushCamera(cameraType, this);
	}

	public void PushCamera(MVCameraBase cameraBase)
	{
		cameraStack.PushCamera(cameraBase, this);
	}

	public void RemoveCamera(CameraType cameraType)
	{
		cameraStack.RemoveCamera(cameraType, this);
	}

	public void RemoveCamera(MVCameraBase cameraBase)
	{
		cameraStack.RemoveCamera(cameraBase, this);
	}

	public void StartTransitionCam(float transitionTime = 2f, bool soft = false)
	{
		cameraStack.StartTransitionCam(this, transitionTime, soft);
	}

	public void CancelTransitionCam()
	{
		cameraStack.CancelTransitionCam();
	}

	public void UpdateCamera()
	{
		cameraStack.UpdateCamera(this);
	}
}
