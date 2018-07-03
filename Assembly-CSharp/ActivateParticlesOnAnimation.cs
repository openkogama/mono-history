using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateParticlesOnAnimation : MonoBehaviour
{
	private enum animationTypes : byte
	{
		None,
		Idle,
		TPose,
		Jump,
		Walk,
		Swim,
		Dead,
		Shake,
		Nod,
		Wave
	}

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

	private void Start()
	{
		BoneAnimation animation = MVGameControllerBase.WOCM.AvatarLocal.Body.Animation;
		animation.OnAnimationChange = (Action<string>)Delegate.Combine(animation.OnAnimationChange, new Action<string>(OnAvatarAnimationChange));
		AvatarLimbManager limbManager = MVGameControllerBase.WOCM.AvatarLocal.LimbManager;
		limbManager.OnEmoteStart = (Action<string>)Delegate.Combine(limbManager.OnEmoteStart, new Action<string>(OnAvatarAnimationChange));
	}

	private void OnAvatarAnimationChange(string newAnimation)
	{
		for (int i = 0; i < activationDataList.Count; i++)
		{
			if (activationDataList[i].activatingAnimation == newAnimation)
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
