using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsButton : MonoBehaviour
{
	[SerializeField]
	private Button button;

	private string key;

	private int value;

	public void Initialize(string key, int value)
	{
		this.key = key;
		this.value = value;
		button.onClick.AddListener(ValueChanged);
	}

	private void ValueChanged()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleSettingChanged handler, BaseEventData data) =>
		{
			handler.OnSettingChanged(key, value);
		});
	}
}
