using UnityEngine;

public class TeamIconScaleWithDistance : MonoBehaviour
{
	[SerializeField]
	private float minDistance = 10f;

	[SerializeField]
	private float maxDistance = 50f;

	private Vector3 scale = new Vector3(1f, 1f, 1f);

	private void Update()
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			float magnitude = (transform.position - MVGameControllerBase.SpawnRoleDataMediatorLocal.Position).magnitude;
			magnitude = Mathf.Clamp(magnitude, minDistance, maxDistance);
			transform.localScale = scale * magnitude / minDistance;
		}
	}
}
