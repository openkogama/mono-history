using System.Linq;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	private int idx;

	private float[] frameTimes = new float[10];

	public bool showFPS = true;

	private void Start()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	private void Update()
	{
		frameTimes[idx] = 1f / Time.deltaTime;
		idx = (idx + 1) % frameTimes.Length;
	}

	private void OnGUI()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected Obj, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (showFPS)
		{
			float num = frameTimes.Min();
			float num2 = frameTimes.Max();
			float num3 = frameTimes.Average();
			GUI.BeginGroup(new Rect(5f, 5f, 80f, 70f), new GUIStyle(GUIStyle.op_Implicit("box")));
			GUI.Label(new Rect(5f, 5f, 80f, 20f), $"Min: {num:0.0}");
			GUI.Label(new Rect(5f, 25f, 80f, 20f), $"Avg: {num3:0.0}");
			GUI.Label(new Rect(5f, 45f, 80f, 20f), $"Max: {num2:0.0}");
			GUI.EndGroup();
		}
	}
}
