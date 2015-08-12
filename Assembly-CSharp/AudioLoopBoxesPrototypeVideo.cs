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
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.transform.localScale = Vector3.one * 20f;
		gameObject.GetComponent<Renderer>().material.mainTexture = textures[2];
		gameObject.GetComponent<Renderer>().material.mainTextureScale = Vector2.one * 100f;
		gameObject.transform.position = new Vector3(99.5f, -0.5f, 99.5f);
		CreateCubeStack(50, 50, new int[5] { 0, 1, 2, 8, 4 });
		CreateCubeStack(60, 100, new int[3] { 13, 14, 16 });
		CreateCubeStack(110, 100, new int[3] { 12, 15, 17 });
		CreateCubeStack(120, 57, new int[3] { 7, 9, 22 });
		CreateCubeStack(95, 45, new int[1] { 18 });
		Debug.Log("Press 'z' to sync loops!");
		StartCoroutine("LoopSyncTimer");
	}

	private void CreateCubeStack(int xPos, int zPos, int[] cubeTypes)
	{
		for (int i = 0; i < cubeTypes.Length; i++)
		{
			GameObject gameObject = new GameObject("cubes");
			cubes[cubeNum] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cubes[cubeNum].transform.parent = gameObject.transform;
			Vector3 position = new Vector3(xPos, i, zPos);
			cubes[cubeNum].transform.position = position;
			cubes[cubeNum].GetComponent<Renderer>().material.mainTexture = textures[cubeTypes[i]];
			cubes[cubeNum].GetComponent<Renderer>().material.mainTextureScale = Vector2.one * 0.5f;
			loopSources[cubeNum] = Object.Instantiate(LoopSource);
			loopSources[cubeNum].transform.parent = gameObject.transform;
			loopSources[cubeNum].transform.position = position;
			loopSources[cubeNum].GetComponent<AudioSource>().clip = loops[cubeTypes[i]];
			loopSources[cubeNum].GetComponent<AudioSource>().Play();
			cubeNum++;
		}
		GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		gameObject2.transform.parent = gameObject2.transform;
		gameObject2.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.5f);
		gameObject2.GetComponent<Renderer>().material.mainTexture = textures[cubeTypes[cubeTypes.Length - 1]];
		gameObject2.GetComponent<Renderer>().material.mainTextureScale = Vector2.one * 0.5f;
		gameObject2.transform.localScale = Vector3.one * 50f;
		gameObject2.transform.position = new Vector3(xPos, (float)cubeTypes.Length * 0.5f, zPos);
		gameObject2.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/VertexLit");
		stackNum++;
	}

	private void Update()
	{
		if (MVInputWrapper.DebugGetKeyDown("z"))
		{
			SyncLoops();
		}
	}

	private void SyncLoops()
	{
		for (int i = 0; i < cubes.Length; i++)
		{
		}
		Debug.Log("Loops synced!");
	}

	private IEnumerator LoopSyncTimer()
	{
		while (true)
		{
			SyncLoops();
			yield return new WaitForSeconds(loopSyncTime);
		}
	}
}
