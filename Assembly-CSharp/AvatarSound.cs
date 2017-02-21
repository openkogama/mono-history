using UnityEngine;

public class AvatarSound : MonoBehaviour
{
	public AudioClip soundTouchParkeur;

	public AudioClip soundTouchBouncy;

	public void HandleWallJump()
	{
		MVGameControllerBase.AudioManager.Play("Parkeur jump", soundTouchParkeur, transform.position, 0.3f, SoundRangeDistance.Short);
	}

	public void HandleActiveBounce()
	{
		MVGameControllerBase.AudioManager.Play("Bounchy jump", soundTouchBouncy, transform.position, 0.3f, SoundRangeDistance.Short);
	}
}
