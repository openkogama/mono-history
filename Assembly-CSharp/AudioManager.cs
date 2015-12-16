using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public class Sound
	{
		public GameObject go;

		public AudioSource audio;
	}

	private const int MaxPooledSoundObjects = 128;

	private GameObject poolTransform;

	private List<Sound> activeSounds = new List<Sound>();

	private List<Sound> pooledSounds = new List<Sound>();

	private List<int> soundsToRemove = new List<int>();

	private void Awake()
	{
		poolTransform = new GameObject();
		poolTransform.name = "SoundPool";
		poolTransform.transform.parent = transform;
		activeSounds.Capacity = 128;
		pooledSounds.Capacity = 128;
		soundsToRemove.Capacity = 128;
		for (int i = 0; i < 128; i++)
		{
			Sound sound = new Sound();
			sound.go = new GameObject();
			sound.go.name = "PooledSoundObject";
			sound.go.SetActive(value: false);
			sound.go.transform.parent = poolTransform.transform;
			sound.audio = sound.go.AddComponent<AudioSource>();
			pooledSounds.Add(sound);
		}
		Object.DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		for (int i = 0; i < activeSounds.Count; i++)
		{
			if (!activeSounds[i].audio.isPlaying)
			{
				soundsToRemove.Add(i);
			}
		}
		for (int num = soundsToRemove.Count; num > 0; num--)
		{
			int index = soundsToRemove[num - 1];
			pooledSounds.Add(activeSounds[index]);
			activeSounds[index].go.SetActive(value: false);
			activeSounds.RemoveAt(index);
		}
		soundsToRemove.Clear();
	}

	public Sound Play(string name, AudioClip clip, Vector3 position, float volume, SoundRangeDistance range)
	{
		if (pooledSounds.Count != 0)
		{
			Sound sound = pooledSounds[pooledSounds.Count - 1];
			pooledSounds.RemoveAt(pooledSounds.Count - 1);
			activeSounds.Add(sound);
			sound.go.SetActive(value: true);
			sound.go.name = "Sound (" + name + ")";
			sound.go.transform.position = position;
			sound.audio.clip = clip;
			sound.audio.volume = volume;
			sound.audio.minDistance = GetMinDistanceFromRange(range);
			sound.audio.maxDistance = GetMaxDistanceFromRange(range);
			sound.audio.Play();
			return sound;
		}
		return null;
	}

	public Sound Play(string name, AudioSource audioSource, Vector3 position)
	{
		if (pooledSounds.Count != 0)
		{
			Sound sound = pooledSounds[pooledSounds.Count - 1];
			pooledSounds.RemoveAt(pooledSounds.Count - 1);
			activeSounds.Add(sound);
			sound.go.SetActive(value: true);
			sound.go.name = "Sound (" + name + ")";
			sound.audio.clip = audioSource.clip;
			sound.audio.volume = audioSource.volume;
			sound.audio.pitch = audioSource.pitch;
			sound.audio.minDistance = audioSource.minDistance;
			sound.audio.maxDistance = audioSource.maxDistance;
			sound.audio.Play();
			return sound;
		}
		return null;
	}

	private static float GetMinDistanceFromRange(SoundRangeDistance range)
	{
		return range switch
		{
			SoundRangeDistance.Short => 5f, 
			SoundRangeDistance.Medium => 7f, 
			SoundRangeDistance.Long => 10f, 
			_ => 10f, 
		};
	}

	private static float GetMaxDistanceFromRange(SoundRangeDistance range)
	{
		return range switch
		{
			SoundRangeDistance.Short => 20f, 
			SoundRangeDistance.Medium => 30f, 
			SoundRangeDistance.Long => 50f, 
			_ => 30f, 
		};
	}
}
