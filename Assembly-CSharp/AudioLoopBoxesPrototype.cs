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
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.transform.localScale = Vector3.one * 20f;
		gameObject.GetComponent<Renderer>().material.mainTexture = textures[2];
		gameObject.GetComponent<Renderer>().material.mainTextureScale = Vector2.one * 100f;
		gameObject.transform.position = new Vector3(99.5f, -0.5f, 99.5f);
		GameObject gameObject2 = new GameObject("cubes");
		for (int i = 0; i < cubes.Length; i++)
		{
			cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cubes[i].transform.parent = gameObject2.transform;
			Vector3 position = new Vector3(Random.Range(0, 199), Random.Range(0, 3), Random.Range(0, 199));
			cubes[i].transform.position = position;
			cubes[i].GetComponent<Renderer>().material.mainTexture = textures[i / 3];
			cubes[i].GetComponent<Renderer>().material.mainTextureScale = Vector2.one * 0.5f;
			loopSources[i] = Object.Instantiate(LoopSource);
			loopSources[i].transform.parent = gameObject2.transform;
			loopSources[i].transform.position = position;
			loopSources[i].GetComponent<AudioSource>().clip = loops[i / 3];
			loopSources[i].GetComponent<AudioSource>().Play();
			for (int j = 0; (float)j < position.y; j++)
			{
				GameObject gameObject3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
				gameObject3.transform.parent = gameObject2.transform;
				gameObject3.GetComponent<Renderer>().material.mainTexture = textures[2];
				gameObject3.GetComponent<Renderer>().material.mainTextureScale = Vector2.one * 0.5f;
				gameObject3.transform.position = new Vector3(position.x, j, position.z);
			}
			GameObject gameObject4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject4.transform.parent = gameObject2.transform;
			gameObject4.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.5f);
			gameObject4.GetComponent<Renderer>().material.mainTexture = textures[i / 3];
			gameObject4.GetComponent<Renderer>().material.mainTextureScale = Vector2.one * 0.5f;
			gameObject4.transform.localScale = Vector3.one * 30f;
			gameObject4.transform.position = position;
			gameObject4.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/VertexLit");
		}
		Debug.Log("Press 'z' to sync loops!");
		StartCoroutine("LoopSyncTimer");
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
			loopSources[i].GetComponent<AudioSource>().time = loopSources[0].GetComponent<AudioSource>().time;
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
