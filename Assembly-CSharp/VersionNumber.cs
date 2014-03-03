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

	private void Update()
	{
		if (MVGameController.Instance.GameJoined)
		{
			((Component)this).gameObject.SetActiveRecursively(false);
		}
	}

	private void Start()
	{
		uxText.Text = "Build " + VersionString;
	}
}
