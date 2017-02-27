using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class BoneAnimation : MonoBehaviour
{
	private float walkMinSpeed = 0.7f;

	private float assumedWalkMaxSpeed = 8f;

	private static readonly int REMOTE_ANIM_SPEEDUP = 20;

	private MVAvatar mvAvatar;

	private AudioSource audioSource;

	private Queue<AnimationData> animationQueue = new Queue<AnimationData>();

	private AnimationData prevAnim;

	private AnimationData currentAnim;

	private AnimationData nextAnim;

	private HashSet<string> playingAnimations = new HashSet<string>();

	private bool pauseNextFrame;

	private int playStartFrame;

	[SerializeField]
	private Animation avatarAnimation;

	private float speed = 1f;

	public AudioSource AudioSource
	{
		get
		{
			if (audioSource == null)
			{
				audioSource = GetComponent<AudioSource>();
			}
			return audioSource;
		}
	}

	public void PlayFootstepAudio()
	{
		if (mvAvatar == null)
		{
			return;
		}
		if (mvAvatar.Avatar.IsLocal)
		{
			if (MVGameControllerBase.WOCM.AvatarLocal.RigidBody.Grounded && !MVGameControllerBase.WOCM.AvatarLocal.IsInVehicle)
			{
				AudioSource.pitch = GetFootstepPitch();
				MVGameControllerBase.AudioManager.Play("Footstep", AudioSource, Camera.main.transform.position + Camera.main.transform.forward);
			}
		}
		else
		{
			AudioSource.pitch = GetFootstepPitch();
			MVGameControllerBase.AudioManager.Play("Footstep", AudioSource, mvAvatar.Body.Transform.position);
		}
	}

	private float GetFootstepPitch()
	{
		return UnityEngine.Random.Range(0.7f, 1.2f);
	}

	public void Attach(MVAvatar mvAvatar, bool isLocal)
	{
		this.mvAvatar = mvAvatar;
		MVRuntimeDataVariable animation = this.mvAvatar.Animation;
		animation.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(animation.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AnimationChangeHandler));
	}

	public void Detach()
	{
		Debug.Log("Detach " + gameObject.name);
		avatarAnimation.Stop();
		MVRuntimeDataVariable animation = mvAvatar.Animation;
		animation.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(animation.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AnimationChangeHandler));
		animationQueue.Clear();
		prevAnim = (currentAnim = (nextAnim = null));
	}

	private void AnimationChangeHandler(object animData)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)animData;
		string state = (string)dictionary["state"];
		int timeStamp = (int)dictionary["timeStamp"];
		if (mvAvatar is MVAvatarLocal)
		{
			currentAnim = new AnimationData(state, timeStamp);
			ComputeAnimation();
		}
		else
		{
			animationQueue.Enqueue(new AnimationData(state, timeStamp));
			ComputeRemoteAnimation();
		}
	}

	public void ComputeBlendAnimation(Dictionary<object, object> animData)
	{
		string state = (string)animData["state"];
		int timeStamp = (int)animData["timeStamp"];
		currentAnim = new AnimationData(state, timeStamp);
		if (prevAnim == null || (prevAnim != null && currentAnim.State != prevAnim.State))
		{
			avatarAnimation.CrossFade(currentAnim.State, 0.3f, PlayMode.StopAll);
			prevAnim = currentAnim;
			currentAnim = null;
		}
	}

	private void OnEnable()
	{
		currentAnim = prevAnim;
		prevAnim = null;
	}

	private void ComputeAnimation()
	{
		if (currentAnim != null && (prevAnim == null || (prevAnim != null && currentAnim.State != prevAnim.State)))
		{
			if (currentAnim.State == "Jump")
			{
				avatarAnimation.Rewind("Jump");
				avatarAnimation.Play(currentAnim.State, PlayMode.StopAll);
			}
			else
			{
				avatarAnimation.CrossFade(currentAnim.State, 0.3f, PlayMode.StopAll);
			}
			float num = 0f;
			if (currentAnim.TimeStamp < TransformNetworkManager.DelayedTime)
			{
				num = 0.001f * (float)(TransformNetworkManager.DelayedTime - currentAnim.TimeStamp) / avatarAnimation[currentAnim.State].length;
				avatarAnimation[currentAnim.State].time = num;
			}
			prevAnim = currentAnim;
			currentAnim = null;
		}
	}

	private void ComputeRemoteAnimation()
	{
		int num = TransformNetworkManager.DelayedTime + REMOTE_ANIM_SPEEDUP;
		if (0 < animationQueue.Count)
		{
			if (nextAnim == null)
			{
				nextAnim = animationQueue.Dequeue();
			}
			if (nextAnim != null && currentAnim == null && nextAnim.TimeStamp <= num)
			{
				currentAnim = nextAnim;
				nextAnim = null;
			}
			while (nextAnim != null && nextAnim.TimeStamp <= num && 0 < animationQueue.Count)
			{
				currentAnim = nextAnim;
				nextAnim = animationQueue.Dequeue();
			}
		}
		else if (nextAnim != null && nextAnim.TimeStamp <= num)
		{
			currentAnim = nextAnim;
			nextAnim = null;
		}
		ComputeAnimation();
	}

	public void Play(string animationName)
	{
		playingAnimations.Add(animationName);
		avatarAnimation[animationName].speed = 1f;
		avatarAnimation[animationName].time = 0f;
		avatarAnimation.Play(animationName, PlayMode.StopAll);
	}

	public void PlayAndPauseAt(string animationName, float time)
	{
		playingAnimations.Add(animationName);
		avatarAnimation.Play(animationName, PlayMode.StopAll);
		avatarAnimation[animationName].time = time;
		avatarAnimation[animationName].speed = 0f;
		pauseNextFrame = true;
		playStartFrame = Time.frameCount;
		avatarAnimation.Sample();
	}

	public float GetAnimationTime(string animation)
	{
		return avatarAnimation[animation].length;
	}

	public void Stop()
	{
		avatarAnimation.Stop();
	}

	public bool IsPlaying(string animationName)
	{
		return avatarAnimation.IsPlaying(animationName);
	}

	private void Update()
	{
		if (mvAvatar is MVAvatarRemote)
		{
			ComputeRemoteAnimation();
		}
		if (pauseNextFrame && Time.frameCount == playStartFrame + 1)
		{
			avatarAnimation.Stop();
			foreach (AnimationState item in avatarAnimation)
			{
				item.speed = 1f;
			}
			pauseNextFrame = false;
		}
		foreach (AnimationState item2 in avatarAnimation)
		{
			if (item2.name == "Walk" && item2.enabled)
			{
				speed = 1f;
				if (mvAvatar != null)
				{
					speed = mvAvatar.Velocity.magnitude;
				}
				if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
				{
					item2.speed = Mathf.Clamp(speed / assumedWalkMaxSpeed, walkMinSpeed, 1f);
				}
			}
			if (playingAnimations.Contains(item2.name) && !item2.enabled)
			{
				playingAnimations.Remove(item2.name);
			}
		}
	}
}
