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
		if ((Object)(object)nextSoundToPlay != (Object)null && !((Component)this).audio.isPlaying)
		{
			((Component)this).audio.clip = nextSoundToPlay;
			((Component)this).audio.Play();
			nextSoundToPlay = null;
		}
	}

	public void HandleWallJump()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		audioManager.Play("Parkeur jump", soundTouchParkeur, ((Component)this).transform.position, 0.8f, SoundRangeDistance.Short);
	}

	public void HandleActiveBounce()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		audioManager.Play("Bounchy jump", soundTouchBouncy, ((Component)this).transform.position, 0.8f, SoundRangeDistance.Short);
	}

	private void HandleOnAvatarAnimationTick(string animation, int pose)
	{
	}

	private void PlaySound(AudioClip clip)
	{
		nextSoundToPlay = clip;
	}
}
