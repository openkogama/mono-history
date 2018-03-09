using UnityEngine;

public class AvatarlateUpdateManager : MonoBehaviour
{
	private MVBody body;

	private AvatarLimbManager limbManager;

	public void Initialize(MVBody body, AvatarLimbManager limbManager)
	{
		this.body = body;
		this.limbManager = limbManager;
	}

	private void LateUpdate()
	{
		limbManager.UpdateLimbRotations();
		body.UpdateBlinking();
	}
}
