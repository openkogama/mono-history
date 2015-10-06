using UnityEngine;

public class AvatarCameraFade : MonoBehaviour
{
	public Transform cameraTfm;

	public float fadeStartBase = 4f;

	public float fadeEndBase = 2f;

	public float fadeStartDistance = 4f;

	public float fadeEndDistance = 2f;

	private Transform avatarTfm;

	public void SetScaleFadeDistance(float scale)
	{
		fadeEndDistance = fadeEndBase;
		fadeStartDistance = fadeStartBase;
		fadeEndDistance *= scale;
		fadeStartDistance *= scale;
	}

	private void Update()
	{
		if (avatarTfm == null)
		{
			if (MVGameController.WOCM.AvatarLocal == null)
			{
				return;
			}
			MVBody body = MVGameController.WOCM.AvatarLocal.Body;
			if (body == null)
			{
				return;
			}
			avatarTfm = body.GameObject.transform;
		}
		float num = Vector3.Distance(avatarTfm.position, cameraTfm.position);
		float setTransparency = Mathf.Clamp01((num - fadeEndDistance) / (fadeStartDistance - fadeEndDistance));
		MVGameController.WOCM.AvatarLocal.SetTransparency = setTransparency;
	}
}
