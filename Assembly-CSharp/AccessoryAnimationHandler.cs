using System;
using System.Collections;
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

	private MVAvatar mvAvatar;

	private bool shouldResetToIdle;

	private void Start()
	{
		Initialize();
	}

	private void OnDestroy()
	{
		if (mvAvatar != null)
		{
			BoneAnimation animation = mvAvatar.Body.Animation;
			animation.OnAnimationChange = (Action<string>)Delegate.Remove(animation.OnAnimationChange, new Action<string>(OnLocalAvatarAnimationChange));
		}
		if (mvAvatar != null)
		{
			AvatarLimbManager limbManager = mvAvatar.LimbManager;
			limbManager.OnEmoteStart = (Action<string>)Delegate.Remove(limbManager.OnEmoteStart, new Action<string>(OnLocalAvatarAnimationChange));
		}
	}

	private void OnLocalAvatarAnimationChange(string newAnimation)
	{
		if (!gameObject.activeInHierarchy || animations.IsPlaying(newAnimation))
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
		if (!HaveAnimationData(animationName))
		{
			if (animationName != "Idle")
			{
				PlayAnimation("Idle");
			}
			return;
		}
		shouldResetToIdle = false;
		ApplyAnimationSpeed(animationName);
		animations.Play(animationName);
		AnimationState animationState = animations[animationName];
		if (animationState.wrapMode == WrapMode.Once)
		{
			StartCoroutine(ResetToIdle(animationState.length / GetAnimationSpeed(animationName)));
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
		Avatar avatar = GetAvatar();
		if (!(avatar == null))
		{
			mvAvatar = avatar.mvAvatar;
			BoneAnimation animation = mvAvatar.Body.Animation;
			animation.OnAnimationChange = (Action<string>)Delegate.Combine(animation.OnAnimationChange, new Action<string>(OnLocalAvatarAnimationChange));
			AvatarLimbManager limbManager = mvAvatar.LimbManager;
			limbManager.OnEmoteStart = (Action<string>)Delegate.Combine(limbManager.OnEmoteStart, new Action<string>(OnLocalAvatarAnimationChange));
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
