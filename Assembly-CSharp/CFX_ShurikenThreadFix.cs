using System.Collections;
using UnityEngine;

public class CFX_ShurikenThreadFix : MonoBehaviour
{
	private ParticleSystem[] systems;

	private void Awake()
	{
		systems = ((Component)this).GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = systems;
		foreach (ParticleSystem val in array)
		{
			val.enableEmission = false;
		}
		((MonoBehaviour)this).StartCoroutine("WaitFrame");
	}

	private IEnumerator WaitFrame()
	{
		yield return null;
		ParticleSystem[] array = systems;
		foreach (ParticleSystem ps in array)
		{
			ps.enableEmission = true;
			ps.Play(true);
		}
	}
}
