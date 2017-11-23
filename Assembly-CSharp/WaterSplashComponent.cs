using MV.Common;
using UnityEngine;

public class WaterSplashComponent : MonoBehaviour
{
	protected IMovable movingObject;

	protected Bounds bounds;

	private Vector3 offset;

	private int waterObjectID;

	public void Initialize(IMovable obj)
	{
		movingObject = obj;
		bounds = obj.Bounds;
		offset = bounds.center - obj.Position;
	}

	protected virtual void Start()
	{
		if (MVGameControllerBase.GameMode == MVGameMode.Play && !MVGameControllerBase.WaterPlaneManager.IsActive)
		{
			enabled = false;
		}
		else
		{
			waterObjectID = MVGameControllerBase.WaterPlaneManager.Splash.NewObjectID;
		}
	}

	protected virtual void Update()
	{
		if (MVGameControllerBase.WaterPlaneManager.IsActive)
		{
			bounds.center = movingObject.Position + offset;
			MVGameControllerBase.WaterPlaneManager.Splash.WaterSplash(bounds, movingObject.Velocity, waterObjectID);
		}
	}
}
