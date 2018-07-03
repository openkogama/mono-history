using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabMenuButton : TabMenuButtonBase
{
	[SerializeField]
	private Text buttonText;

	[SerializeField]
	private Button button;

	public override void Initialize(int tabId, string categoryName)
	{
		buttonText.text = categoryName;
		button.onClick.AddListener(() =>
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (ITabSelected x, BaseEventData y) =>
			{
				x.TabSelected(tabId);
			});
		});
	}

	public override void SetAsSelected()
	{
		ColorBlock colors = button.colors;
		colors.normalColor = colors.pressedColor;
		button.colors = colors;
	}

	public override void SetAsDeselected()
	{
		ColorBlock colors = button.colors;
		colors.normalColor = colors.disabledColor;
		button.colors = colors;
	}
}
