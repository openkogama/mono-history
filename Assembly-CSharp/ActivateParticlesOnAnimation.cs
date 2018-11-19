using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateParticlesOnAnimation : ActivateOnAnimationBase
{
	[Serializable]
	private struct ActivationData
	{
		public string activatingAnimation;

		public float activationDelay;
	}

	[SerializeField]
	private ParticleSystem particles;

	[SerializeField]
	private List<ActivationData> activationDataList;

	public override void OnAvatarAnimationChange(string newAnimation)
	{
		for (int i = 0; i < activationDataList.Count; i++)
		{
			if (activationDataList[i].activatingAnimation == newAnimation && gameObject.activeInHierarchy)
			{
				StartCoroutine(PlayParticles(activationDataList[i].activationDelay));
				break;
			}
		}
	}

	private IEnumerator PlayParticles(float activationDelay)
	{
		float startTime = Time.time;
		while (Time.time < startTime + activationDelay)
		{
			yield return null;
		}
		particles.Play();
	}
}
