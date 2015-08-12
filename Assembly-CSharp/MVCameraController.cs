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

		public void UpdateCamera(MVCameraController cameraController)
		{
			if (TransitionCamera.RotPercentage < 1f)
			{
				CurCamera.UpdateCamera(cameraController, cameraController.transform);
				TransitionCamera.UpdateCamera(cameraController, cameraController.transform);
			}
			else
			{
				CurCamera.UpdateCamera(cameraController, cameraController.transform);
			}
		}

		public void HandleInput(MVCameraController cameraController)
		{
			CurCamera.HandleInput(cameraController);
		}

		private void EnterCamera(MVCameraBase newCamera, MVCameraController cameraController)
		{
			for (int num = activeCameras.Count - 1; num >= 0; num--)
			{
				activeCameras[num].Exit(cameraController);
				cameraController.onIgnoreInputTypes = (EventHandler<OnIgnoreInputTypesArgs>)Delegate.Remove(cameraController.onIgnoreInputTypes, new EventHandler<OnIgnoreInputTypesArgs>(activeCameras[num].camController_onIgnoreInputTypes));
				activeCameras.RemoveAt(num);
			}
			activeCameras.Add(newCamera);
			cameraController.onIgnoreInputTypes = (EventHandler<OnIgnoreInputTypesArgs>)Delegate.Combine(cameraController.onIgnoreInputTypes, new EventHandler<OnIgnoreInputTypesArgs>(CurCamera.camController_onIgnoreInputTypes));
			CurCamera.Enter(cameraController);
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
			for (int num = activeCameras.Count - 1; num >= 0; num--)
			{
				if (activeCameras[num] == cameraBase)
				{
					activeCameras[num].Exit(cameraController);
					cameraController.onIgnoreInputTypes = (EventHandler<OnIgnoreInputTypesArgs>)Delegate.Remove(cameraController.onIgnoreInputTypes, new EventHandler<OnIgnoreInputTypesArgs>(CurCamera.camController_onIgnoreInputTypes));
					activeCameras.RemoveAt(num);
					break;
				}
			}
		}
	}

	private CameraStack cameraStack;

	[SerializeField]
	private List<MVCameraBase> cameraBases = new List<MVCameraBase>();

	[SerializeField]
	private Transform secondaryCamera;

	[SerializeField]
	private Transform tertiaryCamera;

	private bool isLogicRendered;

	public Shader transparentMultiplyColor;

	private static float baseVolume;

	private static bool mute;

	public static Action<bool> OnMuteChange;

	public bool SecondaryCameraActive
	{
		get
		{
			return secondaryCamera.gameObject.activeInHierarchy;
		}
		set
		{
			secondaryCamera.gameObject.SetActive(value);
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

	public bool IsLogicRendered => isLogicRendered;

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
				Debug.Log("Sound off " + baseVolume);
				AudioListener.volume = 0f;
			}
			else
			{
				Debug.Log("Sound on " + baseVolume);
				AudioListener.volume = baseVolume;
			}
			if (OnMuteChange != null)
			{
				OnMuteChange(mute);
			}
		}
	}

	public event EventHandler<OnIgnoreInputTypesArgs> onIgnoreInputTypes;

	public void RenderLogic(bool renderLogic)
	{
		if (renderLogic)
		{
			GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("Logic");
		}
		else
		{
			GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Logic"));
		}
		isLogicRendered = renderLogic;
	}

	public T GetCamera<T>() where T : MVCameraBase
	{
		return cameraStack.GetCamera<T>();
	}

	private void Awake()
	{
		MVGameController.Game.CameraController = this;
		cameraStack = new CameraStack(cameraBases, this);
		baseVolume = AudioListener.volume;
		Debug.Log(baseVolume);
		Mute = false;
	}

	public void Init()
	{
		gameObject.GetComponent<Camera>().cullingMask = ~((1 << LayerMask.NameToLayer("UXElement")) | (1 << LayerMask.NameToLayer("Preview")) | ((1 << LayerMask.NameToLayer("Hidden")) | (1 << LayerMask.NameToLayer("UXElementSecondary"))));
		Camera component = MVGameController.Game.CameraController.GetComponent<Camera>();
		if (MVGameController.GameMode == MVGameMode.Play && (component.cullingMask & LayerMask.NameToLayer("Logic")) == LayerMask.NameToLayer("Logic"))
		{
			gameObject.GetComponent<Camera>().cullingMask -= 1 << LayerMask.NameToLayer("Logic");
		}
	}

	public void Respawn()
	{
		cameraStack.CurCamera.Respawn();
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
		switch (GameDB.GameType)
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

	public void UpdateCamera()
	{
		cameraStack.HandleInput(this);
		cameraStack.UpdateCamera(this);
	}

	public void HandleInput()
	{
	}
}
