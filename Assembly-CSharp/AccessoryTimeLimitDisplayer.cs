using System;
using MV.WorldObject.Accessories;
using UnityEngine;
using UnityEngine.UI;

public class AccessoryTimeLimitDisplayer : MonoBehaviour
{
	[SerializeField]
	private Text timeLeftText;

	private AccessoryTimelimit accessoryTimeLimitData;

	private static readonly string format = "{0:00}:{1:00}:{2:00}";

	public void Initialize(AccessoryTimelimit accessoryData)
	{
		accessoryTimeLimitData = accessoryData;
	}

	private void Update()
	{
		if (accessoryTimeLimitData != null)
		{
			float num = (float)(DateTime.UtcNow - accessoryTimeLimitData.timeLimitStartTime).TotalHours;
			int num2 = accessoryTimeLimitData.timeLimit - Mathf.FloorToInt(num * 3600f);
			if (num2 < 0)
			{
				num2 = 0;
			}
			int num3 = Mathf.FloorToInt((float)num2 / 3600f);
			if (num3 >= 48)
			{
				timeLeftText.text = Mathf.FloorToInt(num3 / 24) + 1 + TM._(" DAYS LEFT!");
			}
			else
			{
				timeLeftText.text = string.Format(format, num3, (float)Mathf.FloorToInt((float)num2 / 60f) % 60f, Mathf.FloorToInt((float)num2 % 60f));
			}
		}
	}
}
