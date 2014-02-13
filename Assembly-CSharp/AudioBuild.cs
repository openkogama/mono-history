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

	public AudioBuild()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
	}

	private void Awake()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		GameObject val = new GameObject();
		val.transform.parent = ((Component)this).transform;
		buildSource = val.AddComponent<AudioSource>();
		buildSource.playOnAwake = false;
		buildSource.panLevel = 0.86f;
		buildSource.minDistance = 20f;
		buildSource.maxDistance = 40f;
		buildSource.rolloffMode = (AudioRolloffMode)1;
	}

	public void CubeAdded(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PlayClip(worldPos, cubeAdded, 0.8f, 1.1f);
	}

	public void CubeRemoved(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PlayClip(worldPos, cubeRemoved, 1f, 1f);
	}

	public void FaceMoved(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PlayClip(worldPos, faceMoved, 0.5f, 1.4f);
	}

	public void EdgeMoved(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PlayClip(worldPos, edgeMoved, 1f, 1f);
	}

	public void VertexMoved(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PlayClip(worldPos, vertexMoved, 1f, 1f);
	}

	public void CubePainted(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PlayClip(worldPos, cubePainted, 1f, 1f);
	}

	public void Translate(float moveValue, bool moveToGridPos, Vector3 worldPos)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (moveToGridPos && (!buildSource.isPlaying || (Object)(object)buildSource.clip == (Object)(object)translateNotGrid))
		{
			((Component)buildSource).transform.position = worldPos;
			buildSource.clip = translateGrid;
			buildSource.pitch = Random.Range(0.7f, 1.1f);
			buildSource.volume = Random.Range(0.8f, 1f);
			buildSource.Play();
			currentTranslateMoveValue = moveValue;
		}
		else if (!moveToGridPos && !buildSource.isPlaying && currentTranslateMoveValue != moveValue)
		{
			((Component)buildSource).transform.position = worldPos;
			buildSource.clip = translateNotGrid;
			buildSource.pitch = Random.Range(0.7f, 1.2f);
			buildSource.volume = Random.Range(0.6f, 1f);
			buildSource.Play();
			currentTranslateMoveValue = moveValue;
		}
	}

	public void PlayClip(Vector3 worldPos, AudioClip audioClip, float randMin, float randMax)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (buildSource.time == 0f)
		{
			((Component)buildSource).transform.position = worldPos;
			buildSource.clip = audioClip;
			buildSource.pitch = Random.Range(randMin, randMax);
			buildSource.volume = Random.Range(0.7f, 1f);
			buildSource.Play();
		}
	}
}
