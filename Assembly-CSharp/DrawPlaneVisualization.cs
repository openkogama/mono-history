using UnityEngine;

internal class DrawPlaneVisualization
{
	private GameObject drawPlaneVisualizationUp;

	private GameObject drawPlaneVisualizationDown;

	private float _zFightingOffset = 0.001f;

	private float baseScale = 100f;

	public bool Active
	{
		set
		{
			drawPlaneVisualizationUp.active = value;
			drawPlaneVisualizationDown.active = value;
		}
	}

	public DrawPlaneVisualization(string resource, float zOffset)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected Obj, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		_zFightingOffset = zOffset;
		drawPlaneVisualizationUp = (GameObject)Object.Instantiate(Resources.Load(resource), Vector3.zero + Vector3.up * _zFightingOffset, Quaternion.identity);
		drawPlaneVisualizationDown = (GameObject)Object.Instantiate(Resources.Load(resource), Vector3.zero - Vector3.up * _zFightingOffset, Quaternion.FromToRotation(Vector3.up, Vector3.down));
		baseScale = drawPlaneVisualizationUp.transform.localScale.x;
	}

	public void Destroy()
	{
		Object.Destroy((Object)(object)drawPlaneVisualizationUp);
		Object.Destroy((Object)(object)drawPlaneVisualizationDown);
	}

	public void SetPos(Vector3 pos)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)drawPlaneVisualizationUp != (Object)null)
		{
			drawPlaneVisualizationUp.transform.position = pos + Vector3.up * _zFightingOffset;
		}
		if ((Object)(object)drawPlaneVisualizationDown != (Object)null)
		{
			drawPlaneVisualizationDown.transform.position = pos - Vector3.up * _zFightingOffset;
		}
	}

	public void SetRot(Quaternion rot)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		drawPlaneVisualizationUp.transform.rotation = Quaternion.identity * rot;
		drawPlaneVisualizationDown.transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.down) * rot;
	}

	public void SetScale(Vector3 scale)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localScale = scale;
		localScale.y = 1f;
		localScale.x = scale.x * baseScale;
		localScale.z = scale.z * baseScale;
		drawPlaneVisualizationUp.gameObject.transform.localScale = localScale;
		drawPlaneVisualizationDown.gameObject.transform.localScale = localScale;
	}

	public void SetLayer(string layer)
	{
		drawPlaneVisualizationUp.layer = LayerMask.NameToLayer(layer);
		drawPlaneVisualizationDown.layer = LayerMask.NameToLayer(layer);
	}
}
