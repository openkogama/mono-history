using System.Collections.Generic;

public class AvatarCamerasWrapper
{
	private Dictionary<CameraType, MVCameraBase> avatarCameras = new Dictionary<CameraType, MVCameraBase>();

	public void Add(MVCameraBase camera)
	{
		avatarCameras.Add(camera.CameraType, camera);
	}

	public List<MVCameraBase> GetCameraBases()
	{
		List<MVCameraBase> list = new List<MVCameraBase>();
		foreach (MVCameraBase value in avatarCameras.Values)
		{
			list.Add(value);
		}
		return list;
	}
}
