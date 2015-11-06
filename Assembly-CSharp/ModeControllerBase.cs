using UnityEngine;

public abstract class ModeControllerBase : MonoBehaviour
{
	public virtual void Initialize()
	{
		MVGameControllerBase.CameraController.Init();
		if (!MVGameControllerBase.IsTouristSession)
		{
			MVGameControllerBase.TimeReward.Init();
		}
	}
}
