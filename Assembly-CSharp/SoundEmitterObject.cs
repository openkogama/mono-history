using UnityEngine;

public class SoundEmitterObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private SoundEmitterActiveCheck soundCheck;

	[SerializeField]
	private AudioSource audioSource;

	public GameObject VisualObject => visualObject;

	public SoundEmitterActiveCheck SoundCheck => soundCheck;

	public AudioSource AudioSource => audioSource;
}
