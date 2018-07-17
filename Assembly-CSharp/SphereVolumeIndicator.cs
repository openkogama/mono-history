using UnityEngine;

public class SphereVolumeIndicator : MonoBehaviour
{
	[Header("Configuration")]
	[SerializeField]
	private float lineDotDensity = 22f;

	[SerializeField]
	private int circleSergmentCount = 8;

	[SerializeField]
	private float lineWidth = 1f;

	[Header("Dependencies")]
	[SerializeField]
	private LineRenderer rangeIndicatorXY;

	[SerializeField]
	private LineRenderer rangeIndicatorYZ;

	[SerializeField]
	private LineRenderer rangeIndicatorZX;

	[SerializeField]
	private Material lineDotMaterial;

	private Material materialCopy;

	protected void Awake()
	{
		CopyMaterial();
	}

	private void CopyMaterial()
	{
		materialCopy = new Material(lineDotMaterial);
		rangeIndicatorXY.material = materialCopy;
		rangeIndicatorYZ.material = materialCopy;
		rangeIndicatorZX.material = materialCopy;
	}

	public void SetRadius(float radius)
	{
		Vector3 vector = new Vector3(0f, radius, 0f);
		Quaternion quaternion = Quaternion.AngleAxis(360f / (float)circleSergmentCount, new Vector3(0f, 0f, 1f));
		int num = circleSergmentCount + 1;
		Vector3[] array = new Vector3[num];
		for (int i = 0; i < num; i++)
		{
			vector = quaternion * vector;
			array[i] = vector;
		}
		rangeIndicatorXY.SetVertexCount(num);
		rangeIndicatorXY.SetPositions(array);
		vector = new Vector3(0f, 0f, radius);
		quaternion = Quaternion.AngleAxis(360f / (float)circleSergmentCount, new Vector3(1f, 0f, 0f));
		for (int j = 0; j < num; j++)
		{
			array[j].z = array[j].x;
			array[j].x = 0f;
		}
		rangeIndicatorYZ.SetVertexCount(num);
		rangeIndicatorYZ.SetPositions(array);
		vector = new Vector3(radius, 0f, 0f);
		quaternion = Quaternion.AngleAxis(360f / (float)circleSergmentCount, new Vector3(0f, 1f, 0f));
		for (int k = 0; k < num; k++)
		{
			array[k].x = array[k].y;
			array[k].y = 0f;
		}
		rangeIndicatorZX.SetVertexCount(num);
		rangeIndicatorZX.SetPositions(array);
		materialCopy.SetTextureScale("_MainTex", new Vector2(lineDotDensity * radius, 1f));
		SetLineWidths(lineWidth);
	}

	private void SetLineWidths(float w)
	{
		rangeIndicatorXY.SetWidth(w, w);
		rangeIndicatorYZ.SetWidth(w, w);
		rangeIndicatorZX.SetWidth(w, w);
	}
}
