using UnityEngine;

public class CollectTheItemLineObject : ObjectPrefab
{
	public LineRenderer lineRenderer;

	protected override void OnValidate()
	{
		lineRenderer = GetComponentInChildren<LineRenderer>();
	}
}
