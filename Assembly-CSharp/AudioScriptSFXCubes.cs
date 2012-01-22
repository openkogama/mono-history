using UnityEngine;

public class AudioScriptSFXCubes : MonoBehaviour
{
	public AudioClip spawn;

	public AudioClip sideDragOut;

	public AudioClip sideDragOutWholeCube;

	public AudioClip sideDragIn;

	public AudioClip sideDragInWholeCube;

	public AudioClip sideDragVanish;

	public AudioClip sideDragCollide;

	public AudioClip sideRelease;

	public AudioClip sideReleaseWholeCube;

	public AudioClip sideReleaseVanish;

	public AudioClip[] edgeDrag;

	public AudioClip[] edgeRelease;

	private AudioSource cubeSFXSource;

	private int cubeLength;

	private void Awake()
	{
		cubeSFXSource = ((Component)this).gameObject.AddComponent<AudioSource>();
		cubeSFXSource.playOnAwake = false;
	}

	public void Spawn()
	{
		cubeSFXSource.clip = spawn;
		cubeSFXSource.Play();
	}

	public void Drag(int newCubeLength)
	{
		cubeSFXSource.volume = (float)(newCubeLength + 3) % 4f / 8f + 0.625f;
		if (newCubeLength == 0)
		{
			cubeSFXSource.clip = sideDragVanish;
		}
		else if (cubeLength < newCubeLength)
		{
			if (newCubeLength % 4 == 0)
			{
				cubeSFXSource.clip = sideDragOutWholeCube;
			}
			else
			{
				cubeSFXSource.clip = sideDragOut;
			}
		}
		else if (cubeLength > newCubeLength)
		{
			if (newCubeLength % 4 == 0)
			{
				cubeSFXSource.clip = sideDragInWholeCube;
			}
			else
			{
				cubeSFXSource.clip = sideDragIn;
			}
		}
		Debug.Log((object)(cubeSFXSource.volume + "  " + cubeSFXSource.clip));
		cubeSFXSource.Play();
		cubeLength = newCubeLength;
	}

	private void Update()
	{
		if (Input.GetKeyDown("1"))
		{
			Drag(1);
		}
		if (Input.GetKeyDown("2"))
		{
			Drag(2);
		}
		if (Input.GetKeyDown("3"))
		{
			Drag(3);
		}
		if (Input.GetKeyDown("4"))
		{
			Drag(4);
		}
		if (Input.GetKeyDown("5"))
		{
			Drag(5);
		}
		if (Input.GetKeyDown("6"))
		{
			Drag(6);
		}
		if (Input.GetKeyDown("7"))
		{
			Drag(7);
		}
		if (Input.GetKeyDown("8"))
		{
			Drag(8);
		}
		if (Input.GetKeyDown("9"))
		{
			Drag(9);
		}
		if (Input.GetKeyDown("0"))
		{
			Drag(0);
		}
	}
}
