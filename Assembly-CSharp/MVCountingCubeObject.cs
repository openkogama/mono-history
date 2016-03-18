using UnityEngine;

public class MVCountingCubeObject : ObjectPrefab
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private MVCountingCubeDigits digitManager;

	public AudioSource AudioSource => audioSource;

	public MVCountingCubeDigits DigitManager => digitManager;

	protected override void OnValidate()
	{
	}
}
