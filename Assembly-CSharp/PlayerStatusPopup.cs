using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerStatusPopup : MonoBehaviour
{
	[SerializeField]
	private Text xpProgress;

	[SerializeField]
	private ProgressBarAndroid progressBar;

	[SerializeField]
	private float duration = 2.5f;

	private float currentTime;

	public void Initialize()
	{
		XPProgressData xPProgressData = MVGameControllerBase.Game.LocalPlayer.XPProgressData;
		float num = xPProgressData.XP - xPProgressData.PrevXP;
		float num2 = xPProgressData.NextXP - xPProgressData.PrevXP;
		string arg = FormatXP(num);
		string arg2 = FormatXP(num2);
		xpProgress.text = $"XP: {arg} / {arg2}";
		progressBar.Progress = num / num2;
	}

	private void Update()
	{
		currentTime += Time.deltaTime;
		if (currentTime >= duration)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Effect);
			});
		}
	}

	private string FormatXP(float amount)
	{
		if (amount < 1000f)
		{
			float num = amount;
			return num + string.Empty;
		}
		if (amount < 1000000f)
		{
			return (amount / 1000f).ToString("F0") + "K";
		}
		return (amount / 1000000f).ToString("F0") + "M";
	}
}
