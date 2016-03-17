using UnityEngine;
using UnityEngine.EventSystems;

public class ClientShopButton : MonoBehaviour
{
	public void OnClick()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IOpenClientShop x, BaseEventData y) =>
		{
			x.Activate(UIPushOption.Blocking);
		});
	}
}
