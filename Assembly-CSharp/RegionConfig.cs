using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class RegionConfig : ScriptableObject
{
	[SerializeField]
	public SentryConfig sentryConfig;

	[SerializeField]
	public StatHatConfig StathatConfig;

	[SerializeField]
	public DebugLogHandlerConfig DebuggerLoggerConfig;

	[SerializeField]
	public PhotonLoggingConfig PhotonLoggingConfig;

	[SerializeField]
	public TestSetup TestSetup;
}
