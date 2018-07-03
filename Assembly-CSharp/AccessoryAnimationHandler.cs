using System;
using System.Collections.Generic;
using UnityEngine;

public class AccessoryAnimationHandler : MonoBehaviour
{
	[Serializable]
	private struct AnimationData
	{
		public string animationName;

		public float animationSpeed;
	}

	[SerializeField]
	private Animation animations;

	[SerializeField]
	private List<AnimationData> animationData;

	private void Start()
	{
		BoneAnimation animation = MVGameControllerBase.WOCM.AvatarLocal.Body.Animation;
		animation.OnAnimationChange = (Action<string>)Delegate.Combine(animation.OnAnimationChange, new Action<string>(OnLocalAvatarAnimationChange));
		AvatarLimbManager limbManager = MVGameControllerBase.WOCM.AvatarLocal.LimbManager;
		limbManager.OnEmoteStart = (Action<string>)Delegate.Combine(limbManager.OnEmoteStart, new Action<string>(OnLocalAvatarAnimationChange));
		ApplyAnimationSpeed("Idle");
		animations.Play("Idle");
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			BoneAnimation animation = MVGameControllerBase.WOCM.AvatarLocal.Body.Animation;
			animation.OnAnimationChange = (Action<string>)Delegate.Remove(animation.OnAnimationChange, new Action<string>(OnLocalAvatarAnimationChange));
		}
		if (MVGameControllerBase.Game != null)
		{
			AvatarLimbManager limbManager = MVGameControllerBase.WOCM.AvatarLocal.LimbManager;
			limbManager.OnEmoteStart = (Action<string>)Delegate.Remove(limbManager.OnEmoteStart, new Action<string>(OnLocalAvatarAnimationChange));
		}
	}

	private void OnLocalAvatarAnimationChange(string newAnimation)
	{
		if (animations.IsPlaying(newAnimation))
		{
			return;
		}
		foreach (AnimationState animation in animations)
		{
			if (animation.name == newAnimation)
			{
				PlayAnimation(newAnimation);
				return;
			}
		}
		PlayAnimation("Idle");
	}

	private void PlayAnimation(string animationName)
	{
		ApplyAnimationSpeed(animationName);
		animations.Play(animationName);
	}

	private void ApplyAnimationSpeed(string animationName)
	{
		for (int i = 0; i < animationData.Count; i++)
		{
			if (animationData[i].animationName == animationName)
			{
				animations[animationName].speed = animationData[i].animationSpeed;
			}
		}
	}
}
