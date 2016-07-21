using UnityEngine;

public class MVTeleporterObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private ParticleSystem objParticleSystem;

	public GameObject visualRoot;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public ParticleSystem ParticleSystem => objParticleSystem;

	protected override void OnValidate()
	{
	}
}
