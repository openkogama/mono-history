using UnityEngine;

public interface IAudioManager
{
	AudioManager.Sound Play(string name, AudioClip clip, Vector3 position, float volume, SoundRangeDistance range);

	AudioManager.Sound Play(string name, AudioSource source, Vector3 position);
}
