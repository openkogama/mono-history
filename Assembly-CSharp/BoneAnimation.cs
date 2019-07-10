using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class BoneAnimation : MonoBehaviour
{
	public Action<string> OnAnimationChange;

	private const int REMOTE_ANIM_SPEEDUP = 20;

	private float walkMinSpeed = 0.7f;

	private float assumedWalkMaxSpeed = 8f;

	private MVAvatar mvAvatar;

	private bool isLocal;

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

	private Camera mainCamera;

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

	private void Start()
	{
		mainCamera = Camera.main;
	}

	public void PlayFootstepAudio()
	{
		if (mvAvatar == null)
		{
			return;
		}
		if (isLocal)
		{
			MVAvatarLocal mVAvatarLocal = (MVAvatarLocal)mvAvatar;
			if (mVAvatarLocal.RigidBody.Grounded && !mVAvatarLocal.IsInVehicle)
			{
				AudioSource.pitch = GetFootstepPitch();
				MVGameControllerBase.AudioManager.Play("Footstep", AudioSource, mainCamera.transform.position + mainCamera.transform.forward);
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
		this.isLocal = isLocal;
	}

	public void Detach()
	{
		Debug.Log("Detach " + gameObject.name);
		avatarAnimation.Stop();
		animationQueue.Clear();
		prevAnim = (currentAnim = (nextAnim = null));
	}

	public void AnimationChangeHandler(object animData)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)animData;
		string newAnimation = (string)dictionary["state"];
		int timeStamp = (int)dictionary["timeStamp"];
		StartAnimation(newAnimation, timeStamp);
	}

	public void StartAnimation(string newAnimation, int timeStamp)
	{
		if (OnAnimationChange != null)
		{
			OnAnimationChange(newAnimation);
		}
		if (isLocal)
		{
			currentAnim = new AnimationData(newAnimation, timeStamp);
			ComputeAnimation();
		}
		else
		{
			animationQueue.Enqueue(new AnimationData(newAnimation, timeStamp));
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
		int num = TransformNetworkManager.DelayedTime + 20;
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
		if (!isLocal)
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
					speed = mvAvatar.VelocityRelative.magnitude;
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
