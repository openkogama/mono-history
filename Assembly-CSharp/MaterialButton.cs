using UnityEngine;
using UnityEngine.EventSystems;

public class MaterialButton : MonoBehaviour
{
	public void Execute()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleMaterial handler, BaseEventData data) =>
		{
			handler.ShowMaterialInventory();
		});
	}
}
