using System;
using UnityEngine;

[Serializable]
public class KoGaMaSettingsContainer : ScriptableObject
{
	private struct CrunchDesc(int data, int numOfBits)
	{
		public short data = (short)data;

		public byte numOfBits = (byte)numOfBits;
	}

	[Header("Settings set by build system")]
	[SerializeField]
	private bool showDebugLogin;

	[Header("Don't change")]
	[SerializeField]
	private TextAsset versionText;

	[SerializeField]
	private int versionBuild;

	[SerializeField]
	private int webCacheInvalidationCode;

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

	public int WebCacheInvalidationCode => webCacheInvalidationCode;

	public string WebCacheInvalidationCodeStr => "?version=" + webCacheInvalidationCode;

	public string VersionGuid => versionGuid;

	public bool ShowDebugLogin => showDebugLogin;

	public string VersionString => VersionMajor + "." + VersionMinor + "." + VersionMicro + "." + versionBuild;

	public string VersionStringNoBuild => VersionMajor + "." + VersionMinor + "." + VersionMicro;

	public string BranchName => branchName;

	public string LatestCommitMessage => latestCommitMessage;

	public string BuildTime => buildTime;

	public void InvalidateWebCache(bool serialize = true)
	{
		DateTime utcNow = DateTime.UtcNow;
		CrunchDesc[] array = new CrunchDesc[6]
		{
			new CrunchDesc(utcNow.Year - 2017, 6),
			new CrunchDesc(utcNow.Month, 4),
			new CrunchDesc(utcNow.Day, 5),
			new CrunchDesc(utcNow.Hour, 5),
			new CrunchDesc(utcNow.Minute, 6),
			new CrunchDesc(utcNow.Second, 6)
		};
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			CrunchDesc crunchDesc = array[i];
			num <<= (int)crunchDesc.numOfBits;
			num += crunchDesc.data;
		}
		webCacheInvalidationCode = num;
	}
}
