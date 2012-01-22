using System.Collections;
using UnityEngine;

public class AudioLoopBoxRadioPrototype : MonoBehaviour
{
	private GameObject[] cubes = new GameObject[25];

	public AudioClip[] loops;

	private AudioSource masterSource = new AudioSource();

	private AudioSource[] loopSources = new AudioSource[25];

	public Texture[] textures = new Texture[25];

	private float upVolLo = 0.3f;

	private float upVolHi = 1f;

	private float downStepMin;

	private float downStepMax = 0.4f;

	private float minTime = 20f;

	private float maxTime = 21f;

	private float minFadeTime = 2f;

	private float maxFadeTime = 20f;

	public AudioLoopBoxRadioPrototype()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected Obj, but got Unknown
	}

	private void Awake()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected Obj, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)4);
		val.transform.localScale = new Vector3(1.1f, 0f, 3.15f);
		val.renderer.material.color = new Color(0.2f, 0.2f, 0.2f);
		val.transform.position = new Vector3(-5.1f, -0.5f, 14.4f);
		GameObject val2 = new GameObject("cubes");
		masterSource = ((Component)this).gameObject.AddComponent<AudioSource>();
		masterSource.playOnAwake = true;
		masterSource.loop = true;
		masterSource.clip = loops[0];
		masterSource.volume = 0f;
		masterSource.Play();
		for (int i = 0; i < loopSources.Length; i++)
		{
			cubes[i] = GameObject.CreatePrimitive((PrimitiveType)3);
			cubes[i].transform.parent = val2.transform;
			cubes[i].transform.localScale = new Vector3(0.1f, 0.1f, 1f);
			cubes[i].renderer.material.mainTextureScale = new Vector2(0.1f, 1f);
			cubes[i].transform.position = new Vector3(-0.05f, 0f, (float)i * 1.2f);
			cubes[i].renderer.material.mainTexture = textures[i];
			loopSources[i] = cubes[i].AddComponent<AudioSource>();
			loopSources[i].panLevel = 0f;
			loopSources[i].playOnAwake = false;
			loopSources[i].loop = true;
			loopSources[i].clip = loops[i];
			loopSources[i].volume = 0f;
		}
		AwakePlay(3, 0.6f);
		AwakePlay(0, 0.4f);
		AwakePlay(10, 0.35f);
		AwakePlay(12, 0.37f);
		AwakePlay(6, 0.23f);
		((MonoBehaviour)this).StartCoroutine("LoopShiftTimer");
	}

	private void AwakePlay(int loopSourceID, float volumeValue)
	{
		loopSources[loopSourceID].volume = volumeValue;
		loopSources[loopSourceID].Play();
		Meter(loopSourceID, volumeValue);
	}

	private void PlayClip(int trackId)
	{
		if (trackId != 0)
		{
			if (loopSources[trackId].isPlaying)
			{
				loopSources[trackId].Stop();
				return;
			}
			loopSources[trackId].time = loopSources[0].time;
			loopSources[trackId].Play();
		}
	}

	private void NewVolumes()
	{
		for (int i = 0; i < loopSources.Length; i++)
		{
			if (loopSources[i].isPlaying && loopSources[i].volume <= 0.05f)
			{
				loopSources[i].Stop();
			}
		}
		for (int j = 0; j < loopSources.Length; j++)
		{
			if (loopSources[j].isPlaying)
			{
				float num = Random.Range(downStepMin, downStepMax);
				if (loopSources[j].volume - num > 0f)
				{
					((MonoBehaviour)this).StartCoroutine(LoopFader(j, loopSources[j].volume - num));
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(LoopFader(j, 0f));
				}
			}
		}
		int num2 = 0;
		for (int k = 0; k < loopSources.Length; k++)
		{
			if (loopSources[k].isPlaying)
			{
				num2++;
			}
		}
		int num3 = Random.Range(1, 20);
		if (num3 > num2)
		{
			bool flag = true;
			while (flag)
			{
				int num4 = Random.Range(0, loopSources.Length);
				if (!loopSources[num4].isPlaying)
				{
					loopSources[num4].time = masterSource.time;
					loopSources[num4].Play();
					((MonoBehaviour)this).StartCoroutine(LoopFader(num4, Random.Range(upVolLo, upVolHi)));
					flag = false;
					num2++;
				}
			}
		}
		num3 = Random.Range(1, 20);
		if (num3 > num2)
		{
			bool flag2 = true;
			while (flag2)
			{
				int num5 = Random.Range(0, loopSources.Length);
				if (!loopSources[num5].isPlaying)
				{
					loopSources[num5].time = masterSource.time;
					loopSources[num5].Play();
					((MonoBehaviour)this).StartCoroutine(LoopFader(num5, Random.Range(upVolLo, upVolHi)));
					flag2 = false;
					num2++;
				}
			}
		}
		num3 = Random.Range(1, 20);
		if (num3 <= num2)
		{
			return;
		}
		bool flag3 = true;
		while (flag3)
		{
			int num6 = Random.Range(0, loopSources.Length);
			if (!loopSources[num6].isPlaying)
			{
				loopSources[num6].time = masterSource.time;
				loopSources[num6].Play();
				((MonoBehaviour)this).StartCoroutine(LoopFader(num6, Random.Range(upVolLo, upVolHi)));
				flag3 = false;
			}
		}
	}

	private IEnumerator LoopFader(int loopSourceID, float fadeToNum)
	{
		float counter = 0f;
		float startFade = loopSources[loopSourceID].volume;
		float fadeSpeed = 1f / Random.Range(minFadeTime, maxFadeTime);
		while (counter < 1f)
		{
			float nowValue = Mathf.Lerp(startFade, fadeToNum, counter);
			loopSources[loopSourceID].volume = nowValue;
			Meter(loopSourceID, nowValue);
			counter += Time.deltaTime * fadeSpeed;
			yield return 0;
		}
		loopSources[loopSourceID].volume = fadeToNum;
	}

	private void Meter(int loopSourceID, float volumeValue)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		float num = volumeValue * 9.9f + 0.1f;
		cubes[loopSourceID].transform.localScale = new Vector3(num, 0.1f, 1f);
		cubes[loopSourceID].renderer.material.mainTextureScale = new Vector2(num, 1f);
		cubes[loopSourceID].transform.position = new Vector3(num * -0.5f, 0f, (float)loopSourceID * 1.2f);
	}

	private IEnumerator LoopShiftTimer()
	{
		while (true)
		{
			NewVolumes();
			yield return (object)new WaitForSeconds(Random.Range(minTime, maxTime));
		}
	}
}
