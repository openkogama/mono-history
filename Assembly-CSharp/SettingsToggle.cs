using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SettingsToggle : MonoBehaviour
{
	[SerializeField]
	private Toggle toggle;

	private string key;

	public void Initialize(string key, bool value)
	{
		this.key = key;
		toggle.isOn = value;
		toggle.onValueChanged.AddListener(ValueChanged);
	}

	private void ValueChanged(bool value)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleSettingChanged handler, BaseEventData data) =>
		{
			handler.OnSettingChanged(key, value);
		});
	}
}
