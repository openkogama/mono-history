using UnityEngine;
using UnityEngine.UI;

public class DayNightCycleController : MonoBehaviour
{
	[Header("Dependencies")]
	[SerializeField]
	private Toggle playToggle;

	[SerializeField]
	private Image playImage;

	[SerializeField]
	private Image pauseImage;

	[SerializeField]
	private DayNightSlider slider;

	[SerializeField]
	private RectTransform trackFill;

	private DayNightCycle cycle;

	public void Initialize(DayNightCycle cycle)
	{
		this.cycle = cycle;
		if (playToggle.isOn == cycle.IsPaused)
		{
			playToggle.isOn = !cycle.IsPaused;
		}
		else
		{
			OnToggle(playToggle.isOn);
		}
	}

	protected void Update()
	{
		slider.SetValue(cycle.TimeOfDay, sendEvent: false);
		UpdateTrackBackground();
	}

	protected void OnDestroy()
	{
		cycle.IsPaused = false;
	}

	public void OnToggle(bool play)
	{
		playImage.enabled = !play;
		pauseImage.enabled = play;
		cycle.IsPaused = !play;
	}

	public void SetSimulationTime()
	{
		playToggle.isOn = false;
		cycle.TimeOfDay = slider.value;
		UpdateTrackBackground();
	}

	private void UpdateTrackBackground()
	{
		trackFill.anchorMax = new Vector2(cycle.TimeOfDay / 100f, 1f);
	}
}
