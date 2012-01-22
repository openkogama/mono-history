using System.Collections;
using UnityEngine;

public class AudioOnOffComponent : MonoBehaviour
{
	public GameObject audioSourcePrefab;

	protected AudioSource onOffAudioSource;

	public bool loop;

	public StartLoop startLoop;

	public AudioClip onClip;

	public float onMinPitch = 0.8f;

	public float onMaxPitch = 1.1f;

	public float onMinVol = 0.7f;

	public float onMaxVol = 0.9f;

	private float fadeNum = 1f;

	public float fadeSpeed = 0.5f;

	private void Awake()
	{
		Object val = Object.Instantiate((Object)(object)audioSourcePrefab);
		onOffAudioSource = ((GameObject)((val is GameObject) ? val : null)).GetComponent<AudioSource>();
		((Component)onOffAudioSource).transform.parent = ((Component)this).transform;
		onOffAudioSource.loop = loop;
	}

	private void Start()
	{
		switch (startLoop)
		{
		case StartLoop.On:
			TurnOn();
			break;
		case StartLoop.Off:
			TurnOff();
			break;
		}
		((MonoBehaviour)this).StartCoroutine(Fader(0f, 1f));
	}

	private IEnumerator Fader(float minEndPoint, float maxEndPoint)
	{
		float counter = 0f;
		float startFade = fadeNum;
		onOffAudioSource.pitch = fadeNum;
		float fadeToNum = Random.Range(minEndPoint, maxEndPoint);
		while (counter < 1f)
		{
			fadeNum = Mathf.Lerp(startFade, fadeToNum, counter);
			MonoBehaviour.print((object)fadeNum);
			onOffAudioSource.pitch = fadeNum;
			counter += Time.deltaTime * fadeSpeed;
			yield return 0;
		}
		fadeNum = fadeToNum;
		onOffAudioSource.pitch = fadeNum;
		MonoBehaviour.print((object)("End: " + fadeNum));
		((MonoBehaviour)this).StartCoroutine(Fader(minEndPoint, maxEndPoint));
	}

	public virtual void TurnOn()
	{
		if ((Object)(object)onClip != (Object)null)
		{
			PlayClip(onOffAudioSource, onClip, loop, onMinPitch, onMaxPitch, onMinVol, onMaxVol);
		}
	}

	public virtual void TurnOff()
	{
	}

	protected void PlayClip(AudioSource aS, AudioClip aC, bool loop, float minPitch, float maxPitch, float minVol, float maxVol)
	{
		aS.clip = aC;
		aS.pitch = Random.Range(minPitch, maxPitch);
		aS.volume = Random.Range(minVol, maxVol);
		aS.Play();
		aS.timeSamples = aS.clip.samples / 2;
		if (loop)
		{
			aS.timeSamples = Random.Range(0, aS.clip.samples);
		}
	}
}
