using UnityEngine;

public class FireObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private ParticleSystem fireParticleSystem;

	[SerializeField]
	private GameObject rangeVisualizationRenderer;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public ParticleSystem ParticleSystem => fireParticleSystem;

	public AudioSource AudioSource => audioSource;

	public GameObject RangeVisualizationRenderer => rangeVisualizationRenderer;
}
