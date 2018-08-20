using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivitySettings : MonoBehaviour
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private InputField inputField;

	[SerializeField]
	private float interval;

	[SerializeField]
	private float mouseSensitivityMaxModifier;

	[SerializeField]
	private float mouseSensitivityMinModifier;

	private const float middleValue = 50f;

	private float mouseSensitivity;

	private void Start()
	{
		mouseSensitivity = MVInputWrapper.MouseSensitivityModifier;
		float value = CalculateSliderValueFromMouseSensitivityValue();
		value = RoundValue(value);
		inputField.text = value.ToString();
		slider.value = value;
	}

	public void SyncMouseSensitivity()
	{
		MVInputWrapper.MouseSensitivityModifier = mouseSensitivity;
		MVGameControllerBase.OperationRequests.SetMouseSensitivity(mouseSensitivity);
	}

	public void SliderValueChanged()
	{
		float value = slider.value;
		value = RoundValue(value);
		inputField.text = value.ToString();
		mouseSensitivity = CalculateMouseSensitivityValueFromValue(value);
	}

	public void InputFieldValueChanged()
	{
		float.TryParse(inputField.text, out var result);
		result = RoundValue(result);
		inputField.text = result.ToString();
		slider.value = result;
		mouseSensitivity = CalculateMouseSensitivityValueFromValue(result);
		SyncMouseSensitivity();
	}

	private float CalculateMouseSensitivityValueFromValue(float value)
	{
		float num = 0f;
		if (value == 50f)
		{
			return 1f;
		}
		if (value < 50f)
		{
			float num2 = 1f - value / 50f;
			return 1f / ((mouseSensitivityMinModifier - 1f) * num2 + 1f);
		}
		float num3 = (value - 50f) / 50f;
		return num3 * (mouseSensitivityMaxModifier - 1f) + 1f;
	}

	private float CalculateSliderValueFromMouseSensitivityValue()
	{
		float num = 0f;
		if (mouseSensitivity == 1f)
		{
			return 50f;
		}
		if (mouseSensitivity < 1f)
		{
			float num2 = 1f - (1f / mouseSensitivity - 1f) / (mouseSensitivityMinModifier - 1f);
			return num2 * 50f;
		}
		float num3 = (mouseSensitivity - 1f) / (mouseSensitivityMaxModifier - 1f);
		return num3 * 50f + 50f;
	}

	private float RoundValue(float value)
	{
		value = Mathf.Round(value / interval) * interval;
		value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
		return value;
	}
}
