using UnityEngine;

public class AudioBuild : MonoBehaviour
{
	public AudioClip cubeAdded;

	public AudioClip cubeRemoved;

	public AudioClip faceMoved;

	public AudioClip edgeMoved;

	public AudioClip vertexMoved;

	public AudioClip cubePainted;

	public AudioClip translateNotGrid;

	public AudioClip translateGrid;

	private AudioSource buildSource = new AudioSource();

	private float currentTranslateMoveValue;

	private void Awake()
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = transform;
		buildSource = gameObject.AddComponent<AudioSource>();
		buildSource.playOnAwake = false;
		buildSource.spatialBlend = 0.86f;
		buildSource.minDistance = 20f;
		buildSource.maxDistance = 40f;
		buildSource.rolloffMode = AudioRolloffMode.Linear;
	}

	public void CubeAdded(Vector3 worldPos)
	{
		PlayClip(worldPos, cubeAdded, 0.8f, 1.1f);
	}

	public void CubeRemoved(Vector3 worldPos)
	{
		PlayClip(worldPos, cubeRemoved, 1f, 1f);
	}

	public void FaceMoved(Vector3 worldPos)
	{
		PlayClip(worldPos, faceMoved, 0.5f, 1.4f);
	}

	public void EdgeMoved(Vector3 worldPos)
	{
		PlayClip(worldPos, edgeMoved, 1f, 1f);
	}

	public void VertexMoved(Vector3 worldPos)
	{
		PlayClip(worldPos, vertexMoved, 1f, 1f);
	}

	public void CubePainted(Vector3 worldPos)
	{
		PlayClip(worldPos, cubePainted, 1f, 1f);
	}

	public void Translate(float moveValue, bool moveToGridPos, Vector3 worldPos)
	{
		if (moveToGridPos && (!buildSource.isPlaying || buildSource.clip == translateNotGrid))
		{
			buildSource.transform.position = worldPos;
			buildSource.clip = translateGrid;
			buildSource.pitch = Random.Range(0.7f, 1.1f);
			buildSource.volume = Random.Range(0.8f, 1f);
			buildSource.Play();
			currentTranslateMoveValue = moveValue;
		}
		else if (!moveToGridPos && !buildSource.isPlaying && currentTranslateMoveValue != moveValue)
		{
			buildSource.transform.position = worldPos;
			buildSource.clip = translateNotGrid;
			buildSource.pitch = Random.Range(0.7f, 1.2f);
			buildSource.volume = Random.Range(0.6f, 1f);
			buildSource.Play();
			currentTranslateMoveValue = moveValue;
		}
	}

	public void PlayClip(Vector3 worldPos, AudioClip audioClip, float randMin, float randMax)
	{
		if (buildSource.time == 0f)
		{
			buildSource.transform.position = worldPos;
			buildSource.clip = audioClip;
			buildSource.pitch = Random.Range(randMin, randMax);
			buildSource.volume = Random.Range(0.7f, 1f);
			buildSource.Play();
		}
	}
}
