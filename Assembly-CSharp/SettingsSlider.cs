using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SettingsSlider : MonoBehaviour
{
	public Slider slider;

	public float interval = 1f;

	[SerializeField]
	private bool round;

	private string key;

	public void Initialize(string key, float value, float minValue, float maxValue)
	{
		this.key = key;
		slider.maxValue = maxValue;
		slider.value = value;
		slider.minValue = minValue;
	}

	public void Initialize(string key, int value, int minValue, int maxValue)
	{
		this.key = key;
		slider.maxValue = maxValue;
		slider.value = value;
		slider.minValue = minValue;
	}

	public void ValueChanged()
	{
		float value = slider.value;
		if (round)
		{
			value = Mathf.Round(slider.value / interval) * interval;
			slider.value = value;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleSettingChanged handler, BaseEventData data) =>
		{
			handler.OnSettingChanged(key, value);
		});
	}

	private void Reset()
	{
		slider = GetComponent<Slider>();
	}
}
