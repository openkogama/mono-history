using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawer : MonoBehaviour
{
	private List<IGizmo> gizmos = new List<IGizmo>();

	public void AddSphere(Vector3 center, float radius, Color color, bool clear = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Add(new SphereGizmo(center, radius, color), clear);
	}

	public void AddBounds(Bounds bounds, Color color, bool clear = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Add(new CubeGizmo(bounds, color), clear);
	}

	public void Add(IGizmo gizmo, bool clear = false)
	{
		if (clear)
		{
			gizmos.Clear();
		}
		gizmos.Add(gizmo);
	}

	private void OnDrawGizmos()
	{
		foreach (IGizmo gizmo in gizmos)
		{
			gizmo.OnDrawGizmos();
		}
	}
}
