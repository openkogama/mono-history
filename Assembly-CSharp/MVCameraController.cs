using System.Collections.Generic;

public class MVCameraController
{
	private class CameraStack
	{
		private readonly List<MVCameraBase> activeCameras = new List<MVCameraBase>();

		private readonly Dictionary<CameraType, MVCameraBase> cameras = new Dictionary<CameraType, MVCameraBase>();

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

		public void UpdateCamera(MVCameraController cameraController, ProtectedTransform protectedTransform)
		{
			CurCamera.UpdateCamera(cameraController, protectedTransform);
		}

		public void SetCamera(CameraType cameraType, MVCameraController cameraController)
		{
			EnterCamera(cameras[cameraType], cameraController);
		}

		public void SetCamera(MVCameraBase newCamera, MVCameraController cameraController)
		{
			EnterCamera(newCamera, cameraController);
		}

		private void EnterCamera(MVCameraBase newCamera, MVCameraController cameraController)
		{
			ClearStack(cameraController);
			activeCameras.Add(newCamera);
			MVGameControllerBase.MainCameraManager.onIgnoreInputTypes += CurCamera.camController_onIgnoreInputTypes;
			CurCamera.Enter(cameraController);
		}

		private void ClearStack(MVCameraController cameraController)
		{
			int num = activeCameras.Count - 1;
			for (int num2 = num; num2 >= 0; num2--)
			{
				activeCameras[num2].Exit(cameraController);
				MVGameControllerBase.MainCameraManager.onIgnoreInputTypes -= activeCameras[num2].camController_onIgnoreInputTypes;
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
			MVGameControllerBase.MainCameraManager.onIgnoreInputTypes += CurCamera.camController_onIgnoreInputTypes;
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
					MVGameControllerBase.MainCameraManager.onIgnoreInputTypes -= CurCamera.camController_onIgnoreInputTypes;
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

	private CameraStack cameraStack;

	public MVCameraBase CurCamera => cameraStack.CurCamera;

	public T GetCamera<T>() where T : MVCameraBase
	{
		return cameraStack.GetCamera<T>();
	}

	public void Initialize(List<MVCameraBase> cameraBases)
	{
		cameraStack = new CameraStack(cameraBases, this);
	}

	public void Respawn()
	{
		cameraStack.CurCamera.Reset();
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

	public void UpdateCamera(ProtectedTransform protectedTransform)
	{
		cameraStack.UpdateCamera(this, protectedTransform);
	}
}
