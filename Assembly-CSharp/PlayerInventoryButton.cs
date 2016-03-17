using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInventoryButton : MonoBehaviour
{
	public void OnClick()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPlayerInventory x, BaseEventData y) =>
		{
			x.Activate(UIPushOption.Blocking);
		});
	}
}
