using UnityEngine;

public class LoadingCubeAndroid : MonoBehaviour
{
	private bool visible = true;

	private bool cameraSetupDone;

	[SerializeField]
	private GameObject cube;

	[SerializeField]
	private Color backgroundColor;

	[SerializeField]
	private LayerMask layerMask;

	[SerializeField]
	private Camera loadingScreenCamera;

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
			if (MVGameControllerBase.JoinState == MVJoinState.Joining && !cameraSetupDone)
			{
				loadingScreenCamera.backgroundColor = backgroundColor;
				loadingScreenCamera.cullingMask = layerMask;
				cameraSetupDone = true;
			}
			cube.transform.Rotate(Vector3.forward, -60f * Time.deltaTime);
		}
	}

	private void SelfDestruct()
	{
		Object.Destroy(gameObject);
	}
}
