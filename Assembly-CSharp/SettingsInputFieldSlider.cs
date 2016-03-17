using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class SettingsInputFieldSlider : MonoBehaviour
{
	[SerializeField]
	private InputField inputField;

	[SerializeField]
	private SettingsSlider settingsSlider;

	[SerializeField]
	private bool round = true;

	private string key;

	public InputField InputField => inputField;

	public Slider Slider => settingsSlider.slider;

	public void Initialize(string key, float value)
	{
		this.key = key;
		inputField.text = GetInputFieldValue(value).ToString();
	}

	public void Initialize(string key, int value)
	{
		this.key = key;
		inputField.text = GetInputFieldValue(value).ToString();
	}

	private void Update()
	{
		if (inputField.isFocused)
		{
			MVInputWrapper.IsShortcutKeysSuppressed = true;
		}
	}

	public void SliderValueChanged()
	{
		inputField.text = GetInputFieldValue(settingsSlider.slider.value).ToString();
		ValueChanged(inputField.text);
	}

	public void InputFieldValueChanged()
	{
		float.TryParse(inputField.text, out var result);
		inputField.text = GetInputFieldValue(result).ToString();
		ValueChanged(inputField.text);
	}

	private void ValueChanged(string value)
	{
		float.TryParse(value, out var floatValue);
		string text = floatValue.ToString();
		if (text.Length > inputField.characterLimit)
		{
			text = text.Remove(inputField.characterLimit);
		}
		inputField.text = text;
		settingsSlider.slider.value = floatValue;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleSettingChanged handler, BaseEventData data) =>
		{
			handler.OnSettingChanged(key, floatValue);
		});
	}

	private void Reset()
	{
		inputField = GetComponent<InputField>();
	}

	private float GetInputFieldValue(float value)
	{
		float value2 = value;
		if (round)
		{
			value2 = Mathf.Round(value / settingsSlider.interval) * settingsSlider.interval;
		}
		return Mathf.Clamp(value2, settingsSlider.slider.minValue, settingsSlider.slider.maxValue);
	}
}
