using System;
using MV.WorldObject.Accessories;
using UnityEngine;
using UnityEngine.UI;

public class AccessoryTimeLimitDisplayer : MonoBehaviour
{
	[SerializeField]
	private Text timeLeftText;

	private AccessoryTimelimit accessoryTimeLimitData;

	private string format = "{0}h {1}m";

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
			timeLeftText.text = string.Format(format, Mathf.FloorToInt((float)num2 / 3600f), (float)Mathf.FloorToInt((float)num2 / 60f) % 60f);
		}
	}
}
