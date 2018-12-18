using MV.Common;
using UnityEngine;

public class AvatarCameraFade : MonoBehaviour
{
	private Vector3 camMoveTowardsOffset = Vector3.zero;

	private float fadeStartDistance = 4f;

	private float fadeEndDistance = 2f;

	public float fadeStartBase = 4f;

	public float fadeEndBase = 2f;

	private float prevDist = -1f;

	private const float mininumDistanceRequired = 0.01f;

	public void SetScaleFadeDistance(float scale)
	{
		fadeEndDistance = fadeEndBase;
		fadeStartDistance = fadeStartBase;
		fadeEndDistance *= scale;
		fadeStartDistance *= scale;
		camMoveTowardsOffset *= scale;
	}

	private void Update()
	{
		if (!MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Hidden))
		{
			Vector3 a = MVGameControllerBase.WOCM.AvatarLocal.Body.Transform.position + camMoveTowardsOffset;
			float num = Vector3.Distance(a, transform.position);
			float num2 = Mathf.Abs(num - prevDist);
			if (num2 > 0.01f)
			{
				prevDist = num;
				float setTransparency = Mathf.Clamp01((num - fadeEndDistance) / (fadeStartDistance - fadeEndDistance));
				MVGameControllerBase.WOCM.AvatarLocal.SetTransparency = setTransparency;
			}
		}
	}

	public void Setup(Vector3 camMoveTowardsOffset, float fadeStartDistance, float fadeEndDistance)
	{
		this.camMoveTowardsOffset = camMoveTowardsOffset;
		this.fadeStartDistance = fadeStartDistance;
		this.fadeEndDistance = fadeEndDistance;
	}
}
