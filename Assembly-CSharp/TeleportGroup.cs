using System;
using UnityEngine;
using UnityEngine.Events;

public class TeleportGroup : MonoBehaviour
{
	public LineRenderer lineRenderer;

	private MVTeleportGroup worldObject;

	public void Initialize(MVTeleportGroup owner)
	{
		worldObject = owner;
		MVTeleporter teleporter = worldObject.Teleporter1;
		teleporter.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(teleporter.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(PositionChanged));
		MVTeleporter teleporter2 = worldObject.Teleporter2;
		teleporter2.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(teleporter2.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(PositionChanged));
		MVTeleportGroup mVTeleportGroup = worldObject;
		mVTeleportGroup.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(mVTeleportGroup.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(PositionChanged));
		lineRenderer.SetPosition(0, worldObject.Teleporter1.WorldPosition);
		lineRenderer.SetPosition(1, worldObject.Teleporter2.WorldPosition);
	}

	private void OnDestroy()
	{
		if (worldObject != null)
		{
			MVTeleporter teleporter = worldObject.Teleporter1;
			teleporter.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Remove(teleporter.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(PositionChanged));
			MVTeleporter teleporter2 = worldObject.Teleporter2;
			teleporter2.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Remove(teleporter2.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(PositionChanged));
		}
	}

	private void PositionChanged(object sender, PositionChangedEventArgs args)
	{
		if (sender == worldObject)
		{
			lineRenderer.SetPosition(0, worldObject.Teleporter1.WorldPosition);
			lineRenderer.SetPosition(1, worldObject.Teleporter2.WorldPosition);
			return;
		}
		int num = -1;
		if (sender == worldObject.Teleporter1)
		{
			num = 0;
		}
		else if (sender == worldObject.Teleporter2)
		{
			num = 1;
		}
		if (num != -1)
		{
			lineRenderer.SetPosition(num, args.NewPos);
		}
	}
}
