using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoryAnimationHandler : ActivateOnAnimationBase
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

	private bool shouldResetToIdle;

	protected override void Start()
	{
		base.Start();
		Initialize();
	}

	public override void OnAvatarAnimationChange(string newAnimation)
	{
		if (!animations.IsPlaying(newAnimation))
		{
			PlayAnimation(newAnimation);
		}
	}

	private Avatar GetAvatar()
	{
		Avatar avatar = null;
		Transform parent = transform.parent;
		while (parent != null)
		{
			avatar = parent.GetComponent<Avatar>();
			parent = parent.parent;
			if (avatar != null)
			{
				break;
			}
		}
		return avatar;
	}

	private bool HaveAnimationData(string animationName)
	{
		for (int i = 0; i < animationData.Count; i++)
		{
			if (animationData[i].animationName == animationName)
			{
				return true;
			}
		}
		return false;
	}

	public void PlayAnimation(string animationName)
	{
		if (gameObject.activeInHierarchy && HaveAnimationData(animationName))
		{
			shouldResetToIdle = false;
			ApplyAnimationSpeed(animationName);
			animations.Play(animationName);
			AnimationState animationState = animations[animationName];
			if (animationState.wrapMode == WrapMode.Once)
			{
				StartCoroutine(ResetToIdle(animationState.length / GetAnimationSpeed(animationName)));
			}
		}
	}

	private void ApplyAnimationSpeed(string animationName)
	{
		animations[animationName].speed = GetAnimationSpeed(animationName);
	}

	public float GetAnimationSpeed(string animationName)
	{
		float result = 1f;
		for (int i = 0; i < animationData.Count; i++)
		{
			if (animationData[i].animationName == animationName)
			{
				result = animationData[i].animationSpeed;
			}
		}
		return result;
	}

	public void Initialize()
	{
		ApplyAnimationSpeed("Idle");
		animations.Play("Idle");
	}

	public void SetAllAnimationToLooping()
	{
		foreach (AnimationState animation in animations)
		{
			animation.wrapMode = WrapMode.Loop;
		}
	}

	private IEnumerator ResetToIdle(float resetDelay)
	{
		shouldResetToIdle = true;
		float startTime = Time.time;
		while (Time.time < startTime + resetDelay && shouldResetToIdle)
		{
			yield return null;
		}
		if (shouldResetToIdle)
		{
			PlayAnimation("Idle");
		}
	}
}
