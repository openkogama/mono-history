using UnityEngine;

public class AvatarlateUpdateManager : MonoBehaviour
{
	private MVBody body;

	private AvatarLimbManager limbManager;

	private bool isInitialized;

	public void Initialize(MVBody body, AvatarLimbManager limbManager)
	{
		this.body = body;
		this.limbManager = limbManager;
		isInitialized = true;
	}

	private void LateUpdate()
	{
		if (isInitialized)
		{
			limbManager.UpdateLimbRotations();
			body.UpdateBlinking();
		}
	}
}
