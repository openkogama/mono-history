using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class BoneAnimation : MonoBehaviour, INetworkUpdateListener
{
	private static readonly int REMOTE_ANIM_SPEEDUP = 20;

	private MVAvatar mvAvatar;

	private MVNetworkListener woListener;

	private Queue<AnimationData> animationQueue = new Queue<AnimationData>();

	private AnimationData prevAnim;

	private AnimationData currentAnim;

	private AnimationData nextAnim;

	private HashSet<string> playingAnimations = new HashSet<string>();

	private bool pauseNextFrame;

	private int playStartFrame;

	public EventHandler<AnimationChangedEventArgs> AnimationChanged = delegate
	{
	};

	public EventHandler<AnimationClipStoppedEventArgs> AnimationClipStopped = delegate
	{
	};

	private void Start()
	{
		if (!GetComponent<Animation>().isPlaying)
		{
			GetComponent<Animation>().Play("Idle", PlayMode.StopAll);
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
			if (GameDB.LocalAvatar.RigidBody.Grounded && !GameDB.LocalAvatar.IsInVehicle)
			{
				GetComponent<AudioSource>().pitch = GetFootstepPitch();
				MVGameController.AudioManager.Play("Footstep", GetComponent<AudioSource>(), Camera.main.transform.position + Camera.main.transform.forward);
			}
		}
		else
		{
			GetComponent<AudioSource>().pitch = GetFootstepPitch();
			MVGameController.AudioManager.Play("Footstep", GetComponent<AudioSource>(), mvAvatar.Body.Transform.position);
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
		if (!isLocal)
		{
			woListener = mvAvatar.NetworkObject as MVNetworkListener;
			woListener.AddNetorkUpdateListener(this);
		}
	}

	public void Detach()
	{
		GetComponent<Animation>().Stop();
		MVRuntimeDataVariable animation = mvAvatar.Animation;
		animation.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(animation.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AnimationChangeHandler));
		if (woListener != null)
		{
			woListener.RmoveNetorkUpdateListener(this);
		}
		nextAnim = null;
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

	private void ComputeAnimation()
	{
		if (currentAnim != null && (prevAnim == null || (prevAnim != null && currentAnim.State != prevAnim.State)))
		{
			if (currentAnim.State == "Jump")
			{
				GetComponent<Animation>().Rewind("Jump");
				GetComponent<Animation>().Play(currentAnim.State, PlayMode.StopAll);
			}
			else
			{
				GetComponent<Animation>().CrossFade(currentAnim.State, 0.3f, PlayMode.StopAll);
			}
			float num = 0f;
			if (woListener != null && currentAnim.TimeStamp < woListener.DelayedTime)
			{
				num = 0.001f * (float)(woListener.DelayedTime - currentAnim.TimeStamp) / GetComponent<Animation>()[currentAnim.State].length;
				GetComponent<Animation>()[currentAnim.State].time = num;
			}
			if (prevAnim != null && AnimationClipStopped != null)
			{
				AnimationClipStoppedEventArgs e = new AnimationClipStoppedEventArgs(prevAnim.State);
				AnimationClipStopped(this, e);
			}
			if (AnimationChanged != null)
			{
				AnimationChangedEventArgs e2 = new AnimationChangedEventArgs(currentAnim.State);
				AnimationChanged(this, e2);
			}
			prevAnim = currentAnim;
			currentAnim = null;
		}
	}

	private void ComputeRemoteAnimation()
	{
		if (woListener == null)
		{
			return;
		}
		int num = woListener.DelayedTime + REMOTE_ANIM_SPEEDUP;
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

	private void NetworkListenerUpdatedHandler(MVNetworkObject sender)
	{
		ComputeRemoteAnimation();
	}

	public void OnNetworkUpdated()
	{
		ComputeRemoteAnimation();
	}

	public void Play(string animationName)
	{
		playingAnimations.Add(animationName);
		GetComponent<Animation>()[animationName].speed = 1f;
		GetComponent<Animation>()[animationName].time = 0f;
		GetComponent<Animation>().Play(animationName, PlayMode.StopAll);
	}

	public void PlayAndPauseAt(string animationName, float time)
	{
		playingAnimations.Add(animationName);
		GetComponent<Animation>().Play(animationName, PlayMode.StopAll);
		GetComponent<Animation>()[animationName].time = time;
		GetComponent<Animation>()[animationName].speed = 0f;
		pauseNextFrame = true;
		playStartFrame = Time.frameCount;
		GetComponent<Animation>().Sample();
	}

	public void CrossFade(string animationName, float fadeTime)
	{
		playingAnimations.Add(animationName);
		GetComponent<Animation>().CrossFade(animationName, fadeTime, PlayMode.StopAll);
	}

	public void Stop()
	{
		GetComponent<Animation>().Stop();
	}

	public bool IsPlaying(string animationName)
	{
		return GetComponent<Animation>().IsPlaying(animationName);
	}

	private void Update()
	{
		if (pauseNextFrame && Time.frameCount == playStartFrame + 1)
		{
			GetComponent<Animation>().Stop();
			foreach (AnimationState item in GetComponent<Animation>())
			{
				item.speed = 1f;
			}
			pauseNextFrame = false;
		}
		foreach (AnimationState item2 in GetComponent<Animation>())
		{
			if (playingAnimations.Contains(item2.name) && !item2.enabled)
			{
				playingAnimations.Remove(item2.name);
				if (AnimationClipStopped != null)
				{
					AnimationClipStoppedEventArgs e = new AnimationClipStoppedEventArgs(item2.name);
					AnimationClipStopped(this, e);
				}
			}
		}
	}
}
