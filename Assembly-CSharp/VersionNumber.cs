using UnityEngine;

public class VersionNumber : MonoBehaviour
{
	public int versionMajor;

	public int versionMinor = 7;

	public int versionMicro = 5;

	public int versionBuild;

	public UXText uxText;

	public UXView uxView;

	public string VersionString => versionMajor + "." + versionMinor + "." + versionMicro + "." + versionBuild;

	private void Awake()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	private void Start()
	{
		uxText.Text = "Build " + VersionString;
	}
}
