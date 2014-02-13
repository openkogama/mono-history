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
				Debug.LogError((object)("worldObjectParent already set " + worldObjectParent));
			}
			worldObjectParent = MVGameController.Instance.WOCM.GetWorldObjectByGoId(((Object)((Component)this).gameObject).GetInstanceID());
			if (worldObjectParent == null)
			{
				Debug.LogError((object)"wo not found on gameObject");
			}
		}
	}
}
