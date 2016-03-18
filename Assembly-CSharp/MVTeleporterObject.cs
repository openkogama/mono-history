using UnityEngine;

public class MVTeleporterObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private ParticleSystem particleSystem;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public ParticleSystem ParticleSystem => particleSystem;

	protected override void OnValidate()
	{
	}
}
