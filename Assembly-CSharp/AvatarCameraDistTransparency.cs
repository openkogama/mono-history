using MV.Common;
using UnityEngine;

public class AvatarCameraDistTransparency
{
	private Vector3 camMoveTowardsOffset = Vector3.zero;

	private float fadeStartDistance = 4f;

	private float fadeEndDistance = 2f;

	public float fadeStartBase = 4f;

	public float fadeEndBase = 2f;

	private float prevDist = -1f;

	private const float mininumDistanceRequired = 0.01f;

	public AvatarCameraDistTransparency(Vector3 camMoveTowardsOffset, float fadeStartDistance, float fadeEndDistance)
	{
		this.camMoveTowardsOffset = camMoveTowardsOffset;
		this.fadeStartDistance = fadeStartDistance;
		this.fadeEndDistance = fadeEndDistance;
	}

	public void SetScaleFadeDistance(float scale)
	{
		fadeEndDistance = fadeEndBase;
		fadeStartDistance = fadeStartBase;
		fadeEndDistance *= scale;
		fadeStartDistance *= scale;
		camMoveTowardsOffset *= scale;
	}

	public void Update(MVAvatarLocal avatarLocal)
	{
		if (!MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Hidden))
		{
			Vector3 a = avatarLocal.Body.Transform.position + camMoveTowardsOffset;
			float num = Vector3.Distance(a, MVGameControllerBase.MainCameraManager.transform.position);
			float num2 = Mathf.Abs(num - prevDist);
			if (num2 > 0.01f)
			{
				prevDist = num;
				float setTransparency = Mathf.Clamp01((num - fadeEndDistance) / (fadeStartDistance - fadeEndDistance));
				avatarLocal.SetTransparency = setTransparency;
			}
		}
	}
}
