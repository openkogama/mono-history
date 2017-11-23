using UnityEngine;

public class ObjectParticleEmitterScript : MonoBehaviour
{
	public ParticleSystem particleSystemPrefab;

	private ParticleSystem particleSystemInstance;

	public void Play()
	{
		if (particleSystemPrefab != null)
		{
			particleSystemInstance = (ParticleSystem)Object.Instantiate(particleSystemPrefab, transform.position, transform.rotation);
			if (particleSystemInstance != null)
			{
				particleSystemInstance.Play();
			}
			else
			{
				Debug.LogError("ParticleSystemInstance is null");
			}
		}
		else
		{
			Debug.LogError("ParticleSystemPrefab is null");
		}
	}
}
