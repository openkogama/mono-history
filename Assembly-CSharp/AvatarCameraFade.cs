using UnityEngine;

public class AvatarCameraFade : MonoBehaviour
{
	private Vector3 camMoveTowardsOffset = Vector3.zero;

	private float fadeStartDistance = 4f;

	private float fadeEndDistance = 2f;

	public float fadeStartBase = 4f;

	public float fadeEndBase = 2f;

	public void SetScaleFadeDistance(float scale)
	{
		fadeEndDistance = fadeEndBase;
		fadeStartDistance = fadeStartBase;
		fadeEndDistance *= scale;
		fadeStartDistance *= scale;
	}

	private void Update()
	{
		Vector3 a = MVGameControllerBase.WOCM.AvatarLocal.Body.Transform.position + camMoveTowardsOffset;
		float num = Vector3.Distance(a, transform.position);
		float setTransparency = Mathf.Clamp01((num - fadeEndDistance) / (fadeStartDistance - fadeEndDistance));
		MVGameControllerBase.WOCM.AvatarLocal.SetTransparency = setTransparency;
	}

	public void Setup(Vector3 camMoveTowardsOffset, float fadeStartDistance, float fadeEndDistance)
	{
		this.camMoveTowardsOffset = camMoveTowardsOffset;
		this.fadeStartDistance = fadeStartDistance;
		this.fadeEndDistance = fadeEndDistance;
	}
}
