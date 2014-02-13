using UnityEngine;

public class TeleportGroup : MonoBehaviour
{
	public LineRenderer lineRenderer;

	private MVTeleportGroup worldObject;

	public void Initialize(MVTeleportGroup owner)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
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
