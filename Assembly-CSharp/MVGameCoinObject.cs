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

	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private RotateLocal rotateLocal;

	public GameObject useInteractionRotator;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public AudioSource AudioSource => audioSource;

	public GreyOutObjectScript PickupItem => pickupItem;

	public ObjectParticleEmitterScript Particles => particles;

	public RotateLocal RotateLocal => rotateLocal;

	public GameObject VisualObject => visualObject;

	protected override void OnValidate()
	{
	}
}
