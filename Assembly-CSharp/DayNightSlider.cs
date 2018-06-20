using UnityEngine.UI;

public class DayNightSlider : Slider
{
	public void SetValue(float val, bool sendEvent)
	{
		Set(val, sendEvent);
	}
}
