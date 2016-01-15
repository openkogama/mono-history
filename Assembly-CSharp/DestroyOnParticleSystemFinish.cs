using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyOnParticleSystemFinish : MonoBehaviour
{
	private IEnumerator Start()
	{
		ParticleSystem system = GetComponent<ParticleSystem>();
		yield return new WaitForSeconds(system.duration);
		while (system.IsAlive(withChildren: true))
		{
			yield return 0;
		}
		Object.Destroy(gameObject);
	}
}
