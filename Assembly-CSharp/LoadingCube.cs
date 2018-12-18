using UnityEngine;

public class LoadingCube : MonoBehaviour
{
	[SerializeField]
	private GameObject cube;

	private bool visible = true;

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
