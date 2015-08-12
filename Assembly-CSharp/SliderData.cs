using UnityEngine;

public class SliderData : DialogData
{
	public float sliderValue;

	public bool setMinMaxValue;

	public float minValue;

	public float maxValue;

	public override void ApplyDataToElement(GameObject element)
	{
		UXSlider component = element.GetComponent<UXSlider>();
		if (!(component == null))
		{
			if (setMinMaxValue)
			{
				component.MinValue = minValue;
				component.MaxValue = maxValue;
			}
			component.Value = sliderValue;
		}
	}
}
