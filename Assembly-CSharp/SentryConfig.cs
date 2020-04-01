using System;
using UnityEngine;

[Serializable]
public struct SentryConfig
{
	[SerializeField]
	public string dns;

	[SerializeField]
	public bool isEnabled;
}
