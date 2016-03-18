using UnityEngine;

public class MVSentryGunObject : ObjectPrefab
{
	[SerializeField]
	private SentryGunScript sentryGunScript;

	[SerializeField]
	private AudioSource audioSource;

	public SentryGunScript SentryGunScript => sentryGunScript;

	public AudioSource AudioSource => audioSource;

	protected override void OnValidate()
	{
	}
}
