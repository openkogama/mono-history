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
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	private void Update()
	{
		List<Sound> list = new List<Sound>();
		foreach (Sound activeSound in activeSounds)
		{
			if (!activeSound.audio.isPlaying)
			{
				activeSound.finished = true;
				Object.Destroy((Object)(object)activeSound.go);
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
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Sound (" + name + ")");
		val.transform.parent = ((Component)this).transform;
		val.transform.position = position;
		AudioSource val2 = val.AddComponent<AudioSource>();
		val2.clip = clip;
		val2.playOnAwake = false;
		val2.volume = volume;
		val2.loop = false;
		val2.rolloffMode = (AudioRolloffMode)0;
		val2.minDistance = GetMinDistanceFromRange(range);
		val2.maxDistance = GetMaxDistanceFromRange(range);
		val2.Play();
		Sound sound = new Sound();
		sound.go = val;
		sound.audio = val2;
		sound.finished = false;
		activeSounds.Add(sound);
		return sound;
	}

	public Sound Play(string name, AudioSource audioSource, Vector3 position)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Sound (" + name + ")");
		val.transform.parent = ((Component)this).transform;
		val.transform.position = position;
		AudioSource val2 = val.AddComponent<AudioSource>();
		val2.clip = audioSource.clip;
		val2.playOnAwake = false;
		val2.volume = audioSource.volume;
		val2.loop = false;
		val2.rolloffMode = audioSource.rolloffMode;
		val2.minDistance = audioSource.minDistance;
		val2.maxDistance = audioSource.maxDistance;
		val2.Play();
		Sound sound = new Sound();
		sound.go = val;
		sound.audio = val2;
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
			SoundRangeDistance.Short => 5f, 
			SoundRangeDistance.Medium => 10f, 
			SoundRangeDistance.Long => 50f, 
			_ => 10f, 
		};
	}
}
