using UnityEngine;

public class DebugConsole : MonoBehaviour
{
	private string logstring = string.Empty;

	private bool showDebug;

	private Vector2 scrollPosition = Vector2.zero;

	private void Start()
	{
		DebugLogHandler.AddLogHandler(LogCallbackHandler);
	}

	private void LogCallbackHandler(string condition, string stackTrace, LogType type)
	{
		string text = logstring;
		logstring = text + type.ToString() + " :  [" + Time.frameCount + "] " + condition + "\n";
	}

	private void Update()
	{
		if (MVInputWrapper.DebugGetKeyUp(KeyCode.Alpha0))
		{
			showDebug = !showDebug;
		}
	}

	private void OnGUI()
	{
		if (showDebug)
		{
			int width = Screen.width;
			int num = Screen.height / 3;
			GUI.BeginGroup(new Rect(0f, 0f, width, num));
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(width - 10), GUILayout.Height(num - 30));
			GUILayout.Label(logstring);
			GUILayout.EndScrollView();
			GUI.EndGroup();
		}
	}
}
