using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class SettingsInputField : MonoBehaviour
{
	public InputField inputField;

	private string key;

	public void Initialize(string key, string value)
	{
		this.key = key;
		inputField.text = value;
		inputField.onValueChanged.AddListener(ValueChanged);
	}

	private void Update()
	{
		if (inputField.isFocused)
		{
			MVInputWrapper.IsShortcutKeysSuppressed = true;
		}
	}

	private void ValueChanged(string value)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleSettingChanged handler, BaseEventData data) =>
		{
			handler.OnSettingChanged(key, value);
		});
	}

	private void Reset()
	{
		inputField = GetComponent<InputField>();
	}
}
