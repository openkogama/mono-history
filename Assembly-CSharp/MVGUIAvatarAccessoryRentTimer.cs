using System;
using Localize;
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
			TextSlotIndex textSlotIndex = TextSlotIndex.Empty;
			if (timeBeforeExpire.TotalDays > 1.0)
			{
				num = (float)timeBeforeExpire.TotalDays;
				textSlotIndex = TextSlotIndex.ForDays;
			}
			else if (timeBeforeExpire.TotalHours > 1.0)
			{
				num = (float)timeBeforeExpire.TotalHours;
				textSlotIndex = TextSlotIndex.ForHours;
			}
			else if (timeBeforeExpire.TotalMinutes > 1.0)
			{
				num = (float)timeBeforeExpire.TotalMinutes;
				textSlotIndex = TextSlotIndex.ForMinutesShort;
			}
			else
			{
				num = Mathf.Ceil((float)timeBeforeExpire.TotalSeconds);
				textSlotIndex = TextSlotIndex.ForSecondsShort;
			}
			string text = $"{num:0.#}";
			ValueInsert valueInsert = new ValueInsert();
			valueInsert.AddString(text);
			string text2 = Localization.Instance.GetTextWithValues(textSlotIndex, valueInsert);
			if (float.Parse(text) == 1f)
			{
				text2 = text2.Substring(0, text2.Length - 1);
			}
			if (timeBeforeExpire.Ticks <= 0)
			{
				text2 = Localization.Instance.GetText(TextSlotIndex.Expired);
			}
			expirationTime.Text = text2;
		}
	}
}
