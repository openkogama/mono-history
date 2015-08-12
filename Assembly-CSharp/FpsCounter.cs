using System.Linq;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	private int idx;

	private float[] frameTimes = new float[10];

	public bool showFPS = true;

	private void Start()
	{
		Object.DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		frameTimes[idx] = 1f / Time.deltaTime;
		idx = (idx + 1) % frameTimes.Length;
		if (Input.GetKey(KeyCode.Alpha8) && Input.GetKeyUp(KeyCode.Alpha9))
		{
			showFPS = !showFPS;
		}
	}

	private void OnGUI()
	{
		if (showFPS)
		{
			float num = frameTimes.Min();
			float num2 = frameTimes.Max();
			float num3 = frameTimes.Average();
			GUI.BeginGroup(new Rect(5f, 5f, 80f, 70f), new GUIStyle("box"));
			GUI.Label(new Rect(5f, 5f, 80f, 20f), $"Min: {num:0.0}");
			GUI.Label(new Rect(5f, 25f, 80f, 20f), $"Avg: {num3:0.0}");
			GUI.Label(new Rect(5f, 45f, 80f, 20f), $"Max: {num2:0.0}");
			GUI.EndGroup();
		}
	}
}
