using System;
using UnityEngine;

public class MVGUIAvatarAccessoryRentTimer : MonoBehaviour
{
	public UXGroup expirationGroup;

	public UXText expirationTime;

	private bool _visible;

	private InventoryExpirationInfo inventoryExpirationInfo;

	public void InitializeRentTimer(InventoryExpirationInfo inventoryExpirationInfo)
	{
		this.inventoryExpirationInfo = inventoryExpirationInfo;
	}

	public void SetVisible(bool visible)
	{
		_visible = visible;
		expirationGroup.SetVisible(visible);
	}

	private void Update()
	{
		if (_visible)
		{
			UpdateExpirationTime();
		}
	}

	private void UpdateExpirationTime()
	{
		if (inventoryExpirationInfo != null)
		{
			float num = 0f;
			TimeSpan timeBeforeExpire = inventoryExpirationInfo.TimeBeforeExpire;
			string empty = string.Empty;
			if (timeBeforeExpire.TotalDays > 1.0)
			{
				num = (float)timeBeforeExpire.TotalDays;
				empty = TM._("{0} days");
			}
			else if (timeBeforeExpire.TotalHours > 1.0)
			{
				num = (float)timeBeforeExpire.TotalHours;
				empty = TM._("{0} hours");
			}
			else if (timeBeforeExpire.TotalMinutes > 1.0)
			{
				num = (float)timeBeforeExpire.TotalMinutes;
				empty = TM._("{0} mins");
			}
			else
			{
				num = Mathf.Ceil((float)timeBeforeExpire.TotalSeconds);
				empty = TM._("{0} secs");
			}
			string text = $"{num:0.#}";
			ValueInsert valueInsert = new ValueInsert();
			valueInsert.AddString(text);
			string text2 = TM.GetTextWithValues(empty, valueInsert);
			if (float.Parse(text) == 1f)
			{
				text2 = text2.Substring(0, text2.Length - 1);
			}
			if (timeBeforeExpire.Ticks <= 0)
			{
				text2 = TM._("Expired");
			}
			expirationTime.Text = text2;
		}
	}
}
