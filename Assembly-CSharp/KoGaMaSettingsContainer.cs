using System;
using UnityEngine;

[Serializable]
public class KoGaMaSettingsContainer : ScriptableObject
{
	[SerializeField]
	[Header("Settings set by build system")]
	private bool showDebugLogin;

	[SerializeField]
	[Header("Don't change")]
	private TextAsset versionText;

	[SerializeField]
	private int versionBuild;

	[SerializeField]
	private int versionStreamingAssets;

	[SerializeField]
	private string versionGuid = string.Empty;

	[SerializeField]
	private string branchName = string.Empty;

	[SerializeField]
	private string latestCommitMessage = string.Empty;

	[SerializeField]
	private string buildTime = string.Empty;

	[SerializeField]
	private KoGaMaSettingsContainer assetReference;

	public int VersionMajor => int.Parse(versionText.text.Substring(0, 1));

	public int VersionMinor => int.Parse(versionText.text.Substring(1, 2));

	public int VersionMicro => int.Parse(versionText.text.Substring(3, 3));

	public int VersionCode => int.Parse(versionText.text);

	public int VersionStreamingAssets => versionStreamingAssets;

	public string VersionGuid => versionGuid;

	public bool ShowDebugLogin => showDebugLogin;

	public string VersionString => VersionMajor + "." + VersionMinor + "." + VersionMicro + "." + versionBuild;

	public string BranchName => branchName;

	public string LatestCommitMessage => latestCommitMessage;

	public string BuildTime => buildTime;
}
