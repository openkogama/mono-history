using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IAudioManager
{
	public class Sound
	{
		public GameObject go;

		public AudioSource audio;

		public bool finished;
	}

	private List<Sound> activeSounds = new List<Sound>();

	private void Awake()
	{
		Object.DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		List<Sound> list = new List<Sound>();
		foreach (Sound activeSound in activeSounds)
		{
			if (!activeSound.audio.isPlaying)
			{
				activeSound.finished = true;
				Object.Destroy(activeSound.go);
				list.Add(activeSound);
			}
		}
		foreach (Sound item in list)
		{
			activeSounds.Remove(item);
		}
	}

	public Sound Play(string name, AudioClip clip, Vector3 position, float volume, SoundRangeDistance range)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Audio/BaseAudioManagerPrefab"));
		gameObject.name = "Sound (" + name + ")";
		gameObject.transform.parent = transform;
		gameObject.transform.position = position;
		AudioSource component = gameObject.GetComponent<AudioSource>();
		component.clip = clip;
		component.volume = volume;
		component.maxDistance = GetMaxDistanceFromRange(range);
		component.Play();
		Sound sound = new Sound();
		sound.go = gameObject;
		sound.audio = component;
		sound.finished = false;
		activeSounds.Add(sound);
		return sound;
	}

	public Sound Play(string name, AudioSource audioSource, Vector3 position)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Audio/BaseAudioManagerPrefab"));
		gameObject.name = "Sound (" + name + ")";
		gameObject.transform.parent = transform;
		gameObject.transform.position = position;
		AudioSource component = gameObject.GetComponent<AudioSource>();
		component.clip = audioSource.clip;
		component.volume = audioSource.volume;
		component.pitch = audioSource.pitch;
		component.maxDistance = audioSource.maxDistance;
		component.Play();
		Sound sound = new Sound();
		sound.go = gameObject;
		sound.audio = component;
		sound.finished = false;
		activeSounds.Add(sound);
		return sound;
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
