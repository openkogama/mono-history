using System;
using UnityEngine;

[Serializable]
public class ExtenderBorderInfo
{
	public ExtenderBorderEnum Border;

	[Tooltip("True if the border allows the extender to be visible.")]
	public bool Enabled = true;

	[Tooltip("Offset in pixels from the edge towards center for the extender to travel along.  This is necessary for images that have spacing and shadows around their edges.")]
	public float Margin;

	[Tooltip("Offset in pixels on the near side of the edge for the extender to travel along.  This is necessary so the extender does not move where the image may have rounded corners.")]
	public float CutoffNear;

	[Tooltip("Offset in pixels on the far side of the edge for the extender to travel along.  This is necessary so the extender does not move where the image may have rounded corners.")]
	public float CutoffFar;
}
