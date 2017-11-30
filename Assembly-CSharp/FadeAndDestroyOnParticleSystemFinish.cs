using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FadeAndDestroyOnParticleSystemFinish : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem system;

	private float fadeCountDown;

	private void Start()
	{
		fadeCountDown = system.duration;
	}

	private void Update()
	{
		if (fadeCountDown > 0f)
		{
			fadeCountDown -= Time.deltaTime;
			if (fadeCountDown <= 0f)
			{
				system.Stop();
			}
		}
		else if (system.particleCount <= 0)
		{
			Object.Destroy(gameObject);
		}
	}
}
