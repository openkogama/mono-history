using UnityEngine;

public class VersionNumber : MonoBehaviour
{
	public int versionMajor;

	public int versionMinor = 7;

	public int versionMicro = 5;

	public int versionBuild;

	public string versionGuid = string.Empty;

	public int versionStreamingAssets;

	public string VersionString => versionMajor + "." + versionMinor + "." + versionMicro + "." + versionBuild;

	private void Start()
	{
		Debug.Log("ClientBuild: " + VersionString);
	}

	public override string ToString()
	{
		return "Build: " + VersionString;
	}
}
