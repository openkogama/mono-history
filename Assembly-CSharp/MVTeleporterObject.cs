using UnityEngine;

public class MVTeleporterObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private ParticleSystem objParticleSystem;

	[SerializeField]
	private TeleporterTintObject tintObject;

	public GameObject visualRoot;

	public GameObject useInteractionRotator;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public ParticleSystem ParticleSystem => objParticleSystem;

	public TeleporterTintObject TintObject => tintObject;

	protected override void OnValidate()
	{
	}
}
