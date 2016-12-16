using UnityEngine;

public class MVComponent : MonoBehaviour
{
	public bool findWorldObjectParent = true;

	protected MVWorldObjectClient worldObjectParent;

	protected virtual void Awake()
	{
		if (findWorldObjectParent)
		{
			FindWorldObjectParent();
		}
	}

	public void FindWorldObjectParent()
	{
		findWorldObjectParent = true;
		if (worldObjectParent != null)
		{
			Debug.LogError("worldObjectParent already set " + worldObjectParent);
		}
		worldObjectParent = MVGameControllerBase.WOCM.GetWorldObjectByGoId(gameObject.GetInstanceID());
		if (worldObjectParent == null)
		{
			Debug.LogError(string.Empty + gameObject.name + ": worldobjectParent not found on gameObject");
		}
	}
}
