using System;
using UnityEngine;

[Serializable]
public struct DebugLogHandlerConfig
{
	[SerializeField]
	public bool useSamplingOnAndroidAndWebGL;

	[SerializeField]
	public bool useProxyLogHandler;

	[SerializeField]
	public int maxLogContextQueueCount;

	[SerializeField]
	public ProxyLogHandlerConfig proxyLogHandlerConfig;
}
