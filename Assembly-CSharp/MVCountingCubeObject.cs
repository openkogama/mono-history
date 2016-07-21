using UnityEngine;

public class MVCountingCubeObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private MVCountingCubeDigits digitManager;

	public AudioSource AudioSource => audioSource;

	public MVCountingCubeDigits DigitManager => digitManager;

	public GameObject VisualObject => visualObject;

	protected override void OnValidate()
	{
	}
}
