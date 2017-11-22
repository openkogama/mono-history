using System.Collections;
using UnityEngine;

public class CFX_ShurikenThreadFix : MonoBehaviour
{
	private ParticleSystem[] systems;

	private void Awake()
	{
		systems = GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = systems;
		foreach (ParticleSystem particleSystem in array)
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = false;
		}
		StartCoroutine("WaitFrame");
	}

	private IEnumerator WaitFrame()
	{
		yield return null;
		ParticleSystem[] array = systems;
		foreach (ParticleSystem particleSystem in array)
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = true;
			particleSystem.Play(withChildren: true);
		}
	}
}
