using UnityEngine;

public class FireObject : MonoBehaviour
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private ParticleSystem particleSystem;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private GameObject rangeVisualizationRenderer;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public ParticleSystem ParticleSystem => particleSystem;

	public AudioSource AudioSource => audioSource;

	public GameObject RangeVisualizationRenderer => rangeVisualizationRenderer;
}
