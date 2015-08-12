using UnityEngine;

public class TeleportGroup : MonoBehaviour
{
	public LineRenderer lineRenderer;

	private MVTeleportGroup worldObject;

	public void Initialize(MVTeleportGroup owner)
	{
		worldObject = owner;
		worldObject.Teleporter1.PositionChanged += PositionChanged;
		worldObject.Teleporter2.PositionChanged += PositionChanged;
		worldObject.PositionChanged += PositionChanged;
		lineRenderer.SetPosition(0, worldObject.Teleporter1.WorldPosition);
		lineRenderer.SetPosition(1, worldObject.Teleporter2.WorldPosition);
	}

	private void OnDestroy()
	{
		if (worldObject != null)
		{
			worldObject.Teleporter1.PositionChanged -= PositionChanged;
			worldObject.Teleporter2.PositionChanged -= PositionChanged;
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
