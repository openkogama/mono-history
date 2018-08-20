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
		foreach (ParticleSystem particleSystem in toScale)
		{
			ParticleSystem.MainModule main = particleSystem.main;
			main.startSizeMultiplier *= Source.main.startSizeMultiplier;
		}
	}
}
