using System;
using System.Collections;
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
		if (!((Component)this).animation.isPlaying)
		{
			((Component)this).animation.Play("Idle", (PlayMode)4);
		}
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
		((Component)this).animation.Stop();
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
		Hashtable hashtable = (Hashtable)animData;
		string state = (string)hashtable["state"];
		int timeStamp = (int)hashtable["timeStamp"];
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
				((Component)this).animation.Rewind("Jump");
				((Component)this).animation.Play(currentAnim.State, (PlayMode)4);
			}
			else
			{
				((Component)this).animation.CrossFade(currentAnim.State, 0.3f, (PlayMode)4);
			}
			float num = 0f;
			if (woListener != null && currentAnim.TimeStamp < woListener.DelayedTime)
			{
				num = 0.001f * (float)(woListener.DelayedTime - currentAnim.TimeStamp) / ((Component)this).animation[currentAnim.State].length;
				((Component)this).animation[currentAnim.State].time = num;
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
		((Component)this).animation[animationName].speed = 1f;
		((Component)this).animation[animationName].time = 0f;
		((Component)this).animation.Play(animationName, (PlayMode)4);
	}

	public void PlayAndPauseAt(string animationName, float time)
	{
		playingAnimations.Add(animationName);
		((Component)this).animation.Play(animationName, (PlayMode)4);
		((Component)this).animation[animationName].time = time;
		((Component)this).animation[animationName].speed = 0f;
		pauseNextFrame = true;
		playStartFrame = Time.frameCount;
		((Component)this).animation.Sample();
	}

	public void CrossFade(string animationName, float fadeTime)
	{
		playingAnimations.Add(animationName);
		((Component)this).animation.CrossFade(animationName, fadeTime, (PlayMode)4);
	}

	public void Stop()
	{
		((Component)this).animation.Stop();
	}

	public bool IsPlaying(string animationName)
	{
		return ((Component)this).animation.IsPlaying(animationName);
	}

	private void Update()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected Obj, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		if (pauseNextFrame && Time.frameCount == playStartFrame + 1)
		{
			((Component)this).animation.Stop();
			foreach (AnimationState item in ((Component)this).animation)
			{
				AnimationState val = item;
				val.speed = 1f;
			}
			pauseNextFrame = false;
		}
		foreach (AnimationState item2 in ((Component)this).animation)
		{
			AnimationState val2 = item2;
			if (playingAnimations.Contains(val2.name) && !val2.enabled)
			{
				playingAnimations.Remove(val2.name);
				if (AnimationClipStopped != null)
				{
					AnimationClipStoppedEventArgs e = new AnimationClipStoppedEventArgs(val2.name);
					AnimationClipStopped(this, e);
				}
			}
		}
	}
}
