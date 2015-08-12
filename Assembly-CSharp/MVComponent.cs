using UnityEngine;

public class MVComponent : MonoBehaviour
{
	public bool findWorldObjectParent = true;

	protected MVWorldObjectClient worldObjectParent;

	private void Awake()
	{
		if (findWorldObjectParent)
		{
			if (worldObjectParent != null)
			{
				Debug.LogError("worldObjectParent already set " + worldObjectParent);
			}
			worldObjectParent = MVGameController.WOCM.GetWorldObjectByGoId(gameObject.GetInstanceID());
			if (worldObjectParent == null)
			{
				Debug.LogError("wo not found on gameObject");
			}
		}
	}
}
