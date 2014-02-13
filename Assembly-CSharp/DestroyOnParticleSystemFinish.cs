using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyOnParticleSystemFinish : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return (object)new WaitForSeconds(((Component)this).particleSystem.duration);
		while (((Component)this).particleSystem.IsAlive(true))
		{
			yield return 0;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
