using MV.Common;
using UnityEngine;

public class WaterSplashComponent : MonoBehaviour
{
	protected IMovable movingObject;

	protected Bounds bounds;

	private Vector3 offset;

	private int waterObjectID;

	public virtual void Initialize(IMovable obj)
	{
		movingObject = obj;
		bounds = obj.Bounds;
		offset = bounds.center - obj.Position;
		enabled = true;
	}

	protected virtual void Start()
	{
		if (MVGameControllerBase.GameMode == MVGameMode.Edit || MVGameControllerBase.WaterPlaneManager.IsActive)
		{
			waterObjectID = MVGameControllerBase.WaterPlaneManager.Splash.NewObjectID;
		}
		enabled = false;
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
