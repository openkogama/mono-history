using UnityEngine;

public class Transformer : MonoBehaviour
{
	public Vector3 localPos;

	private Vector3 worldPos1;

	private Vector3 worldPos2;

	private void Update()
	{
		if (MVInputWrapper.DebugGetKey(KeyCode.Alpha0) && MVInputWrapper.DebugGetKey(KeyCode.Q))
		{
			worldPos1 = transform.TransformPoint(localPos);
			worldPos2 = transform.localToWorldMatrix * localPos;
			Debug.Log(string.Concat("Transform fun: ", worldPos1, " matrix: ", worldPos2));
		}
		DrawArrow(worldPos1);
	}

	private void DrawArrow(Vector3 pos)
	{
		Debug.DrawLine(pos - 0.2f * Vector3.up, pos + 0.2f * Vector3.up, Color.green, 1f);
		Debug.DrawLine(pos - 0.2f * Vector3.right, pos + 0.2f * Vector3.right, Color.green, 1f);
		Debug.DrawLine(pos - 0.2f * Vector3.forward, pos + 0.2f * Vector3.forward, Color.green, 1f);
	}
}
