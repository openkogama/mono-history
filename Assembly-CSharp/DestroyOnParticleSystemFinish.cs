using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyOnParticleSystemFinish : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem system;

	private void Start()
	{
		Object.Destroy(gameObject, system.main.duration + system.main.startLifetime.constant);
	}
}
