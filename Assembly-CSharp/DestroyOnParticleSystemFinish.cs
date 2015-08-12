using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyOnParticleSystemFinish : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(GetComponent<ParticleSystem>().duration);
		while (GetComponent<ParticleSystem>().IsAlive(withChildren: true))
		{
			yield return 0;
		}
		Object.Destroy(gameObject);
	}
}
