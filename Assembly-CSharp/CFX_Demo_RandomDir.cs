using UnityEngine;

public class CFX_Demo_RandomDir : MonoBehaviour
{
	public Vector3 min = new Vector3(0f, 0f, 0f);

	public Vector3 max = new Vector3(0f, 360f, 0f);

	public CFX_Demo_RandomDir()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.eulerAngles = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
	}
}
