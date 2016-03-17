using UnityEngine;

public class LoadingCubeAndroid : MonoBehaviour
{
	private bool visible = true;

	[SerializeField]
	private GameObject cube;

	private void Start()
	{
		Object.DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (MVGameControllerBase.Game != null)
		{
			if (MVGameControllerBase.JoinState == MVJoinState.Playing && visible)
			{
				visible = false;
				SelfDestruct();
			}
			cube.transform.Rotate(Vector3.forward, -60f * Time.deltaTime);
		}
	}

	private void SelfDestruct()
	{
		Object.Destroy(gameObject);
	}
}
