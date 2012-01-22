using System.Collections;
using UnityEngine;

public class AudioLoopBoxesPrototypeVideo : MonoBehaviour
{
	private GameObject[] cubes = new GameObject[75];

	public int loopSyncTime = 30;

	public AudioClip[] loops;

	public GameObject LoopSource;

	private GameObject[] loopSources = new GameObject[75];

	public Texture[] textures = new Texture[25];

	private int cubeNum;

	private int stackNum;

	private void Start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)4);
		val.transform.localScale = Vector3.one * 20f;
		val.renderer.material.mainTexture = textures[2];
		val.renderer.material.mainTextureScale = Vector2.one * 100f;
		val.transform.position = new Vector3(99.5f, -0.5f, 99.5f);
		CreateCubeStack(50, 50, new int[5] { 0, 1, 2, 8, 4 });
		CreateCubeStack(60, 100, new int[3] { 13, 14, 16 });
		CreateCubeStack(110, 100, new int[3] { 12, 15, 17 });
		CreateCubeStack(120, 57, new int[3] { 7, 9, 22 });
		CreateCubeStack(95, 45, new int[1] { 18 });
		Debug.Log((object)"Press 'z' to sync loops!");
		((MonoBehaviour)this).StartCoroutine("LoopSyncTimer");
	}

	private void CreateCubeStack(int xPos, int zPos, int[] cubeTypes)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected Obj, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < cubeTypes.Length; i++)
		{
			GameObject val = new GameObject("cubes");
			cubes[cubeNum] = GameObject.CreatePrimitive((PrimitiveType)3);
			cubes[cubeNum].transform.parent = val.transform;
			Vector3 position = new Vector3((float)xPos, (float)i, (float)zPos);
			cubes[cubeNum].transform.position = position;
			cubes[cubeNum].renderer.material.mainTexture = textures[cubeTypes[i]];
			cubes[cubeNum].renderer.material.mainTextureScale = Vector2.one * 0.5f;
			loopSources[cubeNum] = (GameObject)Object.Instantiate((Object)(object)LoopSource);
			loopSources[cubeNum].transform.parent = val.transform;
			loopSources[cubeNum].transform.position = position;
			loopSources[cubeNum].audio.clip = loops[cubeTypes[i]];
			loopSources[cubeNum].audio.Play();
			cubeNum++;
		}
		GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)0);
		val2.transform.parent = val2.transform;
		val2.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
		val2.renderer.material.mainTexture = textures[cubeTypes[^1]];
		val2.renderer.material.mainTextureScale = Vector2.one * 0.5f;
		val2.transform.localScale = Vector3.one * 50f;
		val2.transform.position = new Vector3((float)xPos, (float)cubeTypes.Length * 0.5f, (float)zPos);
		val2.renderer.material.shader = Shader.Find("Transparent/VertexLit");
		stackNum++;
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
