using UnityEngine;

public class AvatarSound : MonoBehaviour
{
	public AudioClip soundTouchParkeur;

	public AudioClip soundTouchBouncy;

	public AudioClip soundMoveOnSoft;

	public AudioClip soundMoveOnHard;

	public AudioClip soundMoveOnParkeur;

	private AudioClip nextSoundToPlay;

	private AudioManager audioManager;

	private AudioSource audioSource;

	public AudioManager AudioManager
	{
		get
		{
			if (audioManager == null)
			{
				audioManager = Object.FindObjectOfType<AudioManager>();
			}
			return audioManager;
		}
	}

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

	public void Start()
	{
	}

	private void Update()
	{
		if (nextSoundToPlay != null && !AudioSource.isPlaying)
		{
			AudioSource.clip = nextSoundToPlay;
			AudioSource.Play();
			nextSoundToPlay = null;
		}
	}

	public void HandleWallJump()
	{
		AudioManager.Play("Parkeur jump", soundTouchParkeur, transform.position, 0.5f, SoundRangeDistance.Short);
	}

	public void HandleActiveBounce()
	{
		AudioManager.Play("Bounchy jump", soundTouchBouncy, transform.position, 0.5f, SoundRangeDistance.Short);
	}

	private void HandleOnAvatarAnimationTick(string animation, int pose)
	{
	}

	private void PlaySound(AudioClip clip)
	{
		nextSoundToPlay = clip;
	}
}
