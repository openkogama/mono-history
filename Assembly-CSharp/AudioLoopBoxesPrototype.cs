using System.Collections;
using UnityEngine;

public class AudioLoopBoxesPrototype : MonoBehaviour
{
	private GameObject[] cubes = new GameObject[75];

	public int loopSyncTime = 30;

	public AudioClip[] loops;

	public GameObject LoopSource;

	private GameObject[] loopSources = new GameObject[75];

	public Texture[] textures = new Texture[25];

	private void Start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected Obj, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected Obj, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)4);
		val.transform.localScale = Vector3.one * 20f;
		val.renderer.material.mainTexture = textures[2];
		val.renderer.material.mainTextureScale = Vector2.one * 100f;
		val.transform.position = new Vector3(99.5f, -0.5f, 99.5f);
		GameObject val2 = new GameObject("cubes");
		for (int i = 0; i < cubes.Length; i++)
		{
			cubes[i] = GameObject.CreatePrimitive((PrimitiveType)3);
			cubes[i].transform.parent = val2.transform;
			Vector3 position = new Vector3((float)Random.Range(0, 199), (float)Random.Range(0, 3), (float)Random.Range(0, 199));
			cubes[i].transform.position = position;
			cubes[i].renderer.material.mainTexture = textures[i / 3];
			cubes[i].renderer.material.mainTextureScale = Vector2.one * 0.5f;
			loopSources[i] = (GameObject)Object.Instantiate((Object)(object)LoopSource);
			loopSources[i].transform.parent = val2.transform;
			loopSources[i].transform.position = position;
			loopSources[i].audio.clip = loops[i / 3];
			loopSources[i].audio.Play();
			for (int j = 0; (float)j < position.y; j++)
			{
				GameObject val3 = GameObject.CreatePrimitive((PrimitiveType)3);
				val3.transform.parent = val2.transform;
				val3.renderer.material.mainTexture = textures[2];
				val3.renderer.material.mainTextureScale = Vector2.one * 0.5f;
				val3.transform.position = new Vector3(position.x, (float)j, position.z);
			}
			GameObject val4 = GameObject.CreatePrimitive((PrimitiveType)0);
			val4.transform.parent = val2.transform;
			val4.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
			val4.renderer.material.mainTexture = textures[i / 3];
			val4.renderer.material.mainTextureScale = Vector2.one * 0.5f;
			val4.transform.localScale = Vector3.one * 30f;
			val4.transform.position = position;
			val4.renderer.material.shader = Shader.Find("Transparent/VertexLit");
		}
		Debug.Log((object)"Press 'z' to sync loops!");
		((MonoBehaviour)this).StartCoroutine("LoopSyncTimer");
	}

	private void Update()
	{
		if (Input.GetKeyDown("z"))
		{
			SyncLoops();
		}
	}

	private void SyncLoops()
	{
		for (int i = 0; i < cubes.Length; i++)
		{
			loopSources[i].audio.time = loopSources[0].audio.time;
		}
		Debug.Log((object)"Loops synced!");
	}

	private IEnumerator LoopSyncTimer()
	{
		while (true)
		{
			SyncLoops();
			yield return (object)new WaitForSeconds((float)loopSyncTime);
		}
	}
}
