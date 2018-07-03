using System.Collections.Generic;
using UnityEngine;

public class BodyClone : MonoBehaviour
{
	private BodyAccessoriesController bodyAccessoriesController;

	public void Initialize(int bodyWoId, Dictionary<object, object> accessoryData)
	{
		MVBodyObject component = GetComponent<MVBodyObject>();
		if (component == null)
		{
			Debug.LogError("Failed to get MVBodyObject");
			return;
		}
		bodyAccessoriesController = new BodyAccessoriesController(bodyWoId, component.BodyData, accessoryData, isVisible: true);
		bodyAccessoriesController.AccessoryMoveOverride = true;
		bodyAccessoriesController.RefreshAccessories(accessoryData);
	}

	public void RefreshAccessories(Dictionary<object, object> accessoryData)
	{
		bodyAccessoriesController.RefreshAccessories(accessoryData);
	}

	public void Destroy()
	{
		bodyAccessoriesController.Destroy();
	}
}
