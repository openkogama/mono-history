using UnityEngine;
using UnityEngine.EventSystems;

public class DevController : MonoBehaviour
{
	[SerializeField]
	private AddToInventoryDevController addToInventoryPrefab;

	[SerializeField]
	private DeleteWoidController deleteWoidPrefab;

	public void OnAddToInventoryPressed()
	{
		AddToInventoryDevController addToInventory = Object.Instantiate(addToInventoryPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(addToInventory.gameObject, UIPushOption.None, null, UIGroupFlags.InventoryUI);
		});
	}

	public void OnDeleteWoidPressed()
	{
		DeleteWoidController deleteWoid = Object.Instantiate(deleteWoidPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(deleteWoid.gameObject, UIPushOption.None, null, UIGroupFlags.InventoryUI);
		});
		deleteWoid.Initialize();
	}
}
