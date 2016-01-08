using System.Collections.Generic;
using UnityEngine;

public class AudioBank
{
	private class AudioBankSoundImpl : AudioBankSound
	{
		private string id;

		private AudioBank audioBank;

		public AudioClip audioClip;

		public string Id => id;

		public AudioBankSoundImpl(AudioBank audioBank, string id)
		{
			this.audioBank = audioBank;
			this.id = id;
		}

		public override void Play()
		{
			audioBank.Play(this);
		}
	}

	private AudioSource audioSource;

	private Dictionary<string, AudioBankSoundImpl> sounds = new Dictionary<string, AudioBankSoundImpl>();

	private string audioResourcesPath;

	public AudioBank(string audioResourcesPath)
	{
		this.audioResourcesPath = audioResourcesPath;
		GameObject gameObject = new GameObject($"AudioBank ({audioResourcesPath})");
		audioSource = gameObject.AddComponent<AudioSource>();
		Object.DontDestroyOnLoad(gameObject);
	}

	public AudioBankSound GetSound(string id)
	{
		if (!sounds.TryGetValue(id, out var value))
		{
			value = new AudioBankSoundImpl(this, id);
			string path = audioResourcesPath + id;
			value.audioClip = Resources.Load(path, typeof(AudioClip)) as AudioClip;
			if (value.audioClip == null)
			{
				Debug.LogWarning($"Could not load sound '{id}' from 'Resources/{audioResourcesPath}'.");
			}
			sounds[id] = value;
		}
		return value;
	}

	private void Play(AudioBankSoundImpl sound)
	{
		if (sound.audioClip != null)
		{
			audioSource.pitch = Random.Range(0.9f, 1.1f);
		}
		audioSource.volume = Random.Range(0.8f, 1f);
		audioSource.PlayOneShot(sound.audioClip);
	}
}
