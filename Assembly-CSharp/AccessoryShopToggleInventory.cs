using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccessoryShopToggleInventory : MonoBehaviour
{
	[SerializeField]
	private Toggle toggle;

	[SerializeField]
	private GameObject backpackOn;

	[SerializeField]
	private GameObject backpackOff;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Image checkBox;

	[SerializeField]
	private Color toggleOnColor;

	[SerializeField]
	private Color toggleOffColor;

	public void SetBackpackIconIsEnabled(bool enable)
	{
		canvasGroup.alpha = ((!enable) ? 0.5f : 1f);
		canvasGroup.interactable = enable;
	}

	public void OnValueChanged()
	{
		backpackOn.SetActive(toggle.isOn);
		backpackOff.SetActive(!toggle.isOn);
		if (toggle.isOn)
		{
			checkBox.color = toggleOnColor;
		}
		else
		{
			checkBox.color = toggleOffColor;
		}
		if (canvasGroup.interactable)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryInventoryControl x, BaseEventData y) =>
			{
				x.DisplayPurchasableItems(!toggle.isOn);
			});
		}
	}
}
