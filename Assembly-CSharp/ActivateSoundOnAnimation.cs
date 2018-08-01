using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSoundOnAnimation : ActivateOnAnimationBase
{
	[Serializable]
	private struct ActivationData
	{
		public string activatingAnimation;

		public float activationDelay;
	}

	[SerializeField]
	private AudioSource sound;

	[SerializeField]
	private List<ActivationData> activationDataList;

	public override void OnAvatarAnimationChange(string newAnimation)
	{
		for (int i = 0; i < activationDataList.Count; i++)
		{
			if (activationDataList[i].activatingAnimation == newAnimation)
			{
				StartCoroutine(PlaySound(activationDataList[i].activationDelay));
				break;
			}
		}
	}

	private IEnumerator PlaySound(float activationDelay)
	{
		float startTime = Time.time;
		while (Time.time < startTime + activationDelay)
		{
			yield return null;
		}
		sound.Play();
	}
}
