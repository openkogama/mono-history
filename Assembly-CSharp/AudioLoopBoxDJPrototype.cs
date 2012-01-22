using UnityEngine;

public class AudioLoopBoxDJPrototype : MonoBehaviour
{
	private GameObject[] cubes = new GameObject[25];

	private GameObject[] xubes = new GameObject[25];

	public AudioClip[] loops;

	private AudioSource masterSource = new AudioSource();

	private AudioSource[] loopSources = new AudioSource[25];

	public Texture[] textures = new Texture[25];

	public AudioLoopBoxDJPrototype()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected Obj, but got Unknown
	}

	private void Start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected Obj, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)4);
		val.transform.localScale = Vector3.one;
		val.renderer.material.color = new Color(0.045f, 0.125f, 0.02f);
		val.transform.position = new Vector3(4f, -0.5f, 4f);
		GameObject val2 = new GameObject("cubes");
		masterSource = ((Component)this).gameObject.AddComponent<AudioSource>();
		masterSource.playOnAwake = true;
		masterSource.loop = true;
		masterSource.clip = loops[0];
		masterSource.volume = 0f;
		masterSource.Play();
		for (int i = 0; i < cubes.Length; i++)
		{
			cubes[i] = GameObject.CreatePrimitive((PrimitiveType)3);
			cubes[i].transform.parent = val2.transform;
			cubes[i].transform.position = new Vector3((float)(i / 5 * 2), 0f, (float)(i % 5 * 2));
			cubes[i].renderer.material.mainTexture = textures[i];
			xubes[i] = GameObject.CreatePrimitive((PrimitiveType)3);
			xubes[i].transform.parent = val2.transform;
			xubes[i].transform.position = new Vector3((float)(i / 5 * 2), -1f, (float)(i % 5 * 2));
			xubes[i].transform.localScale = Vector3.one * 1.4f;
			xubes[i].renderer.material.color = new Color(0f, 0f, 0f);
			AudioLoopBoxCube audioLoopBoxCube = cubes[i].AddComponent<AudioLoopBoxCube>();
			audioLoopBoxCube.index = i;
			loopSources[i] = cubes[i].AddComponent<AudioSource>();
			loopSources[i].panLevel = 0f;
			loopSources[i].volume = 0.8f;
			loopSources[i].playOnAwake = false;
			loopSources[i].loop = true;
			loopSources[i].clip = loops[i];
		}
		Debug.Log((object)"Press 'z' to sync loops!");
	}

	private void Update()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown("z"))
		{
			SyncLoops();
		}
		Ray val = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit val2 = default;
		if (Input.GetKeyDown((KeyCode)323) && Physics.Raycast(val, ref val2))
		{
			AudioLoopBoxCube component = ((Component)val2.collider).gameObject.GetComponent<AudioLoopBoxCube>();
			if ((Object)(object)component != (Object)null)
			{
				PlayStop(component.index);
			}
		}
	}

	private void PlayStop(int i)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)loopSources[i]).audio.isPlaying)
		{
			((Component)loopSources[i]).audio.Stop();
			cubes[i].transform.position = new Vector3((float)(i / 5 * 2), 0f, (float)(i % 5 * 2));
			xubes[i].transform.position = new Vector3((float)(i / 5 * 2), -1f, (float)(i % 5 * 2));
		}
		else
		{
			((Component)loopSources[i]).audio.time = masterSource.time;
			((Component)loopSources[i]).audio.Play();
			cubes[i].transform.position = new Vector3((float)(i / 5 * 2), -0.8f, (float)(i % 5 * 2));
			xubes[i].transform.position = new Vector3((float)(i / 5 * 2), -2f, (float)(i % 5 * 2));
		}
	}

	private void SyncLoops()
	{
		for (int i = 0; i < cubes.Length; i++)
		{
			((Component)loopSources[i]).audio.time = masterSource.time;
		}
		Debug.Log((object)"Loops synced!");
	}
}
