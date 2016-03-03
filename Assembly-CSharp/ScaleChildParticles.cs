using UnityEngine;

public class ScaleChildParticles : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem Source;

	[SerializeField]
	private ParticleSystem[] ToScale;

	private void Start()
	{
		ParticleSystem[] toScale = ToScale;
		for (int i = 0; i < toScale.Length; i++)
		{
			toScale[i].startSize *= Source.startSize;
		}
	}
}
