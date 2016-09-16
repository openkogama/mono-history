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

	[SerializeField]
	private GameObject visualObject;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public ParticleSystem ParticleSystem => fireParticleSystem;

	public AudioSource AudioSource => audioSource;

	public GameObject RangeVisualizationRenderer => rangeVisualizationRenderer;

	public GameObject VisualObject => visualObject;
}
