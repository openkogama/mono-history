using UnityEngine;

public class AudioLoopBoxDJPrototype : MonoBehaviour
{
	private GameObject[] cubes = new GameObject[25];

	private GameObject[] xubes = new GameObject[25];

	public AudioClip[] loops;

	private AudioSource masterSource = new AudioSource();

	private AudioSource[] loopSources = new AudioSource[25];

	public Texture[] textures = new Texture[25];

	private void Start()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<Renderer>().material.color = new Color(0.045f, 0.125f, 0.02f);
		gameObject.transform.position = new Vector3(4f, -0.5f, 4f);
		GameObject gameObject2 = new GameObject("cubes");
		masterSource = base.gameObject.AddComponent<AudioSource>();
		masterSource.playOnAwake = true;
		masterSource.loop = true;
		masterSource.clip = loops[0];
		masterSource.volume = 0f;
		masterSource.Play();
		for (int i = 0; i < cubes.Length; i++)
		{
			cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cubes[i].transform.parent = gameObject2.transform;
			cubes[i].transform.position = new Vector3(i / 5 * 2, 0f, i % 5 * 2);
			cubes[i].GetComponent<Renderer>().material.mainTexture = textures[i];
			xubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			xubes[i].transform.parent = gameObject2.transform;
			xubes[i].transform.position = new Vector3(i / 5 * 2, -1f, i % 5 * 2);
			xubes[i].transform.localScale = Vector3.one * 1.4f;
			xubes[i].GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f);
			AudioLoopBoxCube audioLoopBoxCube = cubes[i].AddComponent<AudioLoopBoxCube>();
			audioLoopBoxCube.index = i;
			loopSources[i] = cubes[i].AddComponent<AudioSource>();
			loopSources[i].spatialBlend = 0f;
			loopSources[i].volume = 0.8f;
			loopSources[i].playOnAwake = false;
			loopSources[i].loop = true;
			loopSources[i].clip = loops[i];
		}
		Debug.Log("Press 'z' to sync loops!");
	}

	private void Update()
	{
		if (MVInputWrapper.DebugGetKeyDown("z"))
		{
			SyncLoops();
		}
		Ray ray = Camera.main.ScreenPointToRay(MVInputWrapper.GetPointerPosition());
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Mouse0) && Physics.Raycast(ray, out var hitInfo))
		{
			AudioLoopBoxCube component = hitInfo.collider.gameObject.GetComponent<AudioLoopBoxCube>();
			if (component != null)
			{
				PlayStop(component.index);
			}
		}
	}

	private void PlayStop(int i)
	{
		if (loopSources[i].GetComponent<AudioSource>().isPlaying)
		{
			loopSources[i].GetComponent<AudioSource>().Stop();
			cubes[i].transform.position = new Vector3(i / 5 * 2, 0f, i % 5 * 2);
			xubes[i].transform.position = new Vector3(i / 5 * 2, -1f, i % 5 * 2);
		}
		else
		{
			loopSources[i].GetComponent<AudioSource>().time = masterSource.time;
			loopSources[i].GetComponent<AudioSource>().Play();
			cubes[i].transform.position = new Vector3(i / 5 * 2, -0.8f, i % 5 * 2);
			xubes[i].transform.position = new Vector3(i / 5 * 2, -2f, i % 5 * 2);
		}
	}

	private void SyncLoops()
	{
		for (int i = 0; i < cubes.Length; i++)
		{
			loopSources[i].GetComponent<AudioSource>().time = masterSource.time;
		}
		Debug.Log("Loops synced!");
	}
}
