using UnityEngine;

public class MVGamePointChestObject : ObjectPrefab
{
	[SerializeField]
	private ObjectParticleEmitterScript particles;

	[SerializeField]
	private GamePointChestModelController modelSelector;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private GameObject visualObject;

	public GameObject useInteractionRotator;

	public ObjectParticleEmitterScript Particles => particles;

	public GamePointChestModelController ModelSelector => modelSelector;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public AudioSource AudioSource => audioSource;

	public GameObject VisualObject => visualObject;

	protected override void OnValidate()
	{
	}
}
