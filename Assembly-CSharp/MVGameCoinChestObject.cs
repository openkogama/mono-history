using UnityEngine;

public class MVGameCoinChestObject : ObjectPrefab
{
	[SerializeField]
	private ObjectParticleEmitterScript particles;

	[SerializeField]
	private GameCoinChestModelSelector modelSelector;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private GameObject visualObject;

	public ObjectParticleEmitterScript Particles => particles;

	public GameCoinChestModelSelector ModelSelector => modelSelector;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public AudioSource AudioSource => audioSource;

	public GameObject VisualObject => visualObject;

	protected override void OnValidate()
	{
	}
}
