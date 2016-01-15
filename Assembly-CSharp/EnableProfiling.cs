using UnityEngine;

public class EnableProfiling : MonoBehaviour
{
	private bool enabledProfiling;

	public bool IsProfilingEnabled => enabledProfiling;

	private void Awake()
	{
		Profiler.logFile = "perfLog.log";
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Alpha8) && Input.GetKeyUp(KeyCode.Alpha1))
		{
			ChangeState();
		}
	}

	private void ChangeState()
	{
		enabledProfiling = !enabledProfiling;
		Debug.Log("Profiling is now " + ((!IsProfilingEnabled) ? "disabled" : "enabled"));
		Profiler.enabled = IsProfilingEnabled;
		Profiler.enableBinaryLog = IsProfilingEnabled;
	}
}
