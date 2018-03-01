using System;
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
	private GameObject visualObject;

	[SerializeField]
	private Collider fireCollider;

	[SerializeField]
	private AnimationCurve soundIntensityScale;

	public Action OnFireObjectCreated;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public ParticleSystem ParticleSystem => fireParticleSystem;

	public AudioSource AudioSource => audioSource;

	public Collider FireCollider => fireCollider;

	public GameObject VisualObject => visualObject;

	public AnimationCurve SoundIntensityScale => soundIntensityScale;

	private void OnEnable()
	{
		if (OnFireObjectCreated != null)
		{
			OnFireObjectCreated();
		}
	}
}
