using UnityEngine;

public class MVGameCoinObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private GreyOutObjectScript pickupItem;

	[SerializeField]
	private ObjectParticleEmitterScript particles;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public AudioSource AudioSource => audioSource;

	public GreyOutObjectScript PickupItem => pickupItem;

	public ObjectParticleEmitterScript Particles => particles;

	protected override void OnValidate()
	{
	}
}
