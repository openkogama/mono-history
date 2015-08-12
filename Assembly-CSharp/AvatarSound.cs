using UnityEngine;

public class AvatarSound : MonoBehaviour
{
	public AudioClip soundTouchParkeur;

	public AudioClip soundTouchBouncy;

	public AudioClip soundMoveOnSoft;

	public AudioClip soundMoveOnHard;

	public AudioClip soundMoveOnParkeur;

	private AudioClip nextSoundToPlay;

	private IAudioManager audioManager;

	public void Start()
	{
		audioManager = Object.FindObjectOfType(typeof(AudioManager)) as AudioManager;
	}

	private void Update()
	{
		if (nextSoundToPlay != null && !GetComponent<AudioSource>().isPlaying)
		{
			GetComponent<AudioSource>().clip = nextSoundToPlay;
			GetComponent<AudioSource>().Play();
			nextSoundToPlay = null;
		}
	}

	public void HandleWallJump()
	{
		audioManager.Play("Parkeur jump", soundTouchParkeur, transform.position, 0.5f, SoundRangeDistance.Short);
	}

	public void HandleActiveBounce()
	{
		audioManager.Play("Bounchy jump", soundTouchBouncy, transform.position, 0.5f, SoundRangeDistance.Short);
	}

	private void HandleOnAvatarAnimationTick(string animation, int pose)
	{
	}

	private void PlaySound(AudioClip clip)
	{
		nextSoundToPlay = clip;
	}
}
