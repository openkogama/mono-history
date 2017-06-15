using UnityEngine;

public class SoundEmitterObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private AudioSource audioSource;

	public GameObject VisualObject => visualObject;

	public AudioSource AudioSource => audioSource;
}
