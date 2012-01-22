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

	private string audioResourcesPath;

	private Dictionary<string, AudioBankSoundImpl> sounds = new Dictionary<string, AudioBankSoundImpl>();

	public AudioBank(string audioResourcesPath)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		this.audioResourcesPath = audioResourcesPath;
		GameObject val = new GameObject($"AudioBank ({audioResourcesPath})");
		audioSource = val.AddComponent<AudioSource>();
		Object.DontDestroyOnLoad((Object)(object)val);
	}

	public AudioBankSound GetSound(string id)
	{
		if (!sounds.TryGetValue(id, out var value))
		{
			value = new AudioBankSoundImpl(this, id);
			string text = audioResourcesPath + id;
			AudioBankSoundImpl audioBankSoundImpl = value;
			Object val = Resources.Load(text, typeof(AudioClip));
			audioBankSoundImpl.audioClip = (AudioClip)(object)((val is AudioClip) ? val : null);
			if ((Object)(object)value.audioClip == (Object)null)
			{
				Debug.LogWarning((object)$"Could not load sound '{id}' from 'Resources/{audioResourcesPath}'.");
			}
			sounds[id] = value;
		}
		return value;
	}

	private void Play(AudioBankSoundImpl sound)
	{
		if ((Object)(object)sound.audioClip != (Object)null)
		{
			audioSource.pitch = Random.Range(0.9f, 1.1f);
		}
		audioSource.volume = Random.Range(0.8f, 1f);
		audioSource.PlayOneShot(sound.audioClip);
	}
}
