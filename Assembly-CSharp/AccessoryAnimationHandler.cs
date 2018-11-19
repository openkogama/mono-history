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

		public string TransitionToAnimationName;
	}

	[SerializeField]
	private Animation animations;

	[SerializeField]
	private List<AnimationData> animationData;

	private string currentCrossFadedAnimation = string.Empty;

	private bool shouldTransitionToNewAnimation;

	private const float crossfadeDuration = 0.2f;

	protected override void Start()
	{
		base.Start();
		Initialize();
	}

	public override void OnAvatarAnimationChange(string newAnimation)
	{
		if (!(currentCrossFadedAnimation == newAnimation))
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
		if (!gameObject.activeInHierarchy)
		{
			return;
		}
		bool flag = HaveAnimationData(animationName);
		if (!flag && animationName != "Idle")
		{
			PlayAnimation("Idle");
			return;
		}
		if (!flag && animationName == "Idle")
		{
			HandleNoIdleAnimation();
			return;
		}
		shouldTransitionToNewAnimation = false;
		ApplyAnimationSpeed(animationName);
		StartCrossfading(animationName);
		AnimationState animationState = animations[animationName];
		if (animationState.wrapMode != WrapMode.Once)
		{
			return;
		}
		for (int i = 0; i < animationData.Count; i++)
		{
			if (animationData[i].animationName == animationName && animationData[i].TransitionToAnimationName != string.Empty)
			{
				StartCoroutine(TransitionToNewAnimation(animationState.length / GetAnimationSpeed(animationName), animationData[i].TransitionToAnimationName));
			}
		}
	}

	private void ApplyAnimationSpeed(string animationName)
	{
		if (animations.GetClip(animationName) != null)
		{
			animations[animationName].speed = GetAnimationSpeed(animationName);
		}
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
		if (animations.GetClip("Idle") != null)
		{
			ApplyAnimationSpeed("Idle");
			animations.Play("Idle");
		}
	}

	public void SetAllAnimationToLooping()
	{
		foreach (AnimationState animation in animations)
		{
			animation.wrapMode = WrapMode.Loop;
		}
	}

	private IEnumerator TransitionToNewAnimation(float resetDelay, string transitionToAnimationName)
	{
		shouldTransitionToNewAnimation = true;
		float startTime = Time.time;
		while (Time.time < startTime + resetDelay && shouldTransitionToNewAnimation)
		{
			yield return null;
		}
		if (shouldTransitionToNewAnimation)
		{
			StartTransitioning(transitionToAnimationName);
		}
	}

	private void StartTransitioning(string transitionToAnimationName)
	{
		ApplyAnimationSpeed(transitionToAnimationName);
		animations.Play(transitionToAnimationName);
		animations[transitionToAnimationName].time = animations[transitionToAnimationName].length / GetAnimationSpeed(transitionToAnimationName);
	}

	private void StartCrossfading(string animationName)
	{
		currentCrossFadedAnimation = animationName;
		animations.CrossFadeQueued(animationName, 0.2f, QueueMode.PlayNow);
	}

	private void HandleNoIdleAnimation()
	{
		if (!(currentCrossFadedAnimation == string.Empty))
		{
			animations.Play(currentCrossFadedAnimation);
			StartCoroutine(StopAnimationNextFrame());
		}
	}

	private IEnumerator StopAnimationNextFrame()
	{
		bool hasFramePassed = false;
		while (!hasFramePassed)
		{
			hasFramePassed = true;
			yield return null;
		}
		animations.Stop();
		currentCrossFadedAnimation = string.Empty;
	}
}
