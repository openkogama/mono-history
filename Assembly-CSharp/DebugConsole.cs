using System;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
	private string logstring = string.Empty;

	private bool showDebug;

	private Vector2 scrollPosition = Vector2.zero;

	public DebugConsole()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		DebugLogHandler.AddLogHandler(LogCallbackHandler);
	}

	private void LogCallbackHandler(string condition, string stackTrace, LogType type)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		string text = logstring;
		logstring = text + ((Enum)type).ToString() + " :  [" + Time.frameCount + "] " + condition + "\n";
	}

	private void Update()
	{
		if (Input.GetKeyUp((KeyCode)48))
		{
			showDebug = !showDebug;
		}
	}

	private void OnGUI()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (showDebug)
		{
			int width = Screen.width;
			int num = Screen.height / 3;
			GUI.BeginGroup(new Rect(0f, 0f, (float)width, (float)num));
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUILayoutOption[2]
			{
				GUILayout.Width((float)(width - 10)),
				GUILayout.Height((float)(num - 30))
			});
			GUILayout.Label(logstring, new GUILayoutOption[0]);
			GUILayout.EndScrollView();
			GUI.EndGroup();
		}
	}
}
