using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeGunSettings : MonoBehaviour
{
	private int woID;

	private MaterialsController materialsController;

	public void Initialize(int woID, MaterialsController materialsController)
	{
		this.woID = woID;
		this.materialsController = materialsController;
		materialsController.ShowMaterialInventory();
		materialsController.materialChange = (UnityAction<byte>)Delegate.Combine(materialsController.materialChange, new UnityAction<byte>(MaterialChange));
		materialsController.materialsPop = (UnityAction)Delegate.Combine(materialsController.materialsPop, new UnityAction(Pop));
	}

	private void Pop()
	{
		Destroy();
	}

	private void MaterialChange(byte materialId)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		Dictionary<object, object> data = worldObjectClient.Data;
		((Dictionary<object, object>)data["itemData"])["material"] = materialId;
		MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(worldObjectClient.Id, data);
		Destroy();
	}

	private void Destroy()
	{
		MaterialsController materialsController = this.materialsController;
		materialsController.materialsPop = (UnityAction)Delegate.Remove(materialsController.materialsPop, new UnityAction(Pop));
		MaterialsController materialsController2 = this.materialsController;
		materialsController2.materialChange = (UnityAction<byte>)Delegate.Remove(materialsController2.materialChange, new UnityAction<byte>(MaterialChange));
		UnityEngine.Object.Destroy(gameObject);
	}
}
