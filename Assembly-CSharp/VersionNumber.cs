using UnityEngine;

public class VersionNumber : MonoBehaviour
{
	public int versionMajor;

	public int versionMinor = 7;

	public int versionMicro = 5;

	public int versionBuild;

	public string versionGuid = string.Empty;

	public int versionStreamingAssets;

	[SerializeField]
	private TextAsset versionText;

	public string VersionString => versionMajor + "." + versionMinor + "." + versionMicro + "." + versionBuild;

	public int VersionCode => int.Parse(versionText.text);

	public void Initialize()
	{
		Start();
	}

	private void Start()
	{
		LogType filterLogType = Debug.logger.filterLogType;
		Debug.logger.filterLogType = LogType.Log;
		string text = versionText.text;
		Debug.Log(text);
		versionMajor = int.Parse(text.Substring(0, 1));
		versionMinor = int.Parse(text.Substring(1, 2));
		versionMicro = int.Parse(text.Substring(3, 3));
		Debug.Log(Application.bundleIdentifier);
		Debug.Log("ClientBuild: " + VersionString);
		Debug.logger.filterLogType = filterLogType;
	}

	public override string ToString()
	{
		return "Build: " + VersionString;
	}
}
