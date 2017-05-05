using System;
using UnityEngine;

public class RollingNumberCounterAndroid : MonoBehaviour
{
	[SerializeField]
	private RollingNumberDigitAndroid[] digits;

	public void SetCounter(int value)
	{
		int num = digits.Length;
		string text = value.ToString();
		if (text.Length > num)
		{
			for (int i = 0; i < num; i++)
			{
				digits[i].Number = 9;
			}
			return;
		}
		if (text.Length < num)
		{
			string text2 = string.Empty;
			for (int j = 0; j < num - text.Length; j++)
			{
				text2 += "0";
			}
			text = text2 + text;
		}
		for (int k = 0; k < num; k++)
		{
			digits[k].Number = Convert.ToInt32(text.Substring(k, 1));
		}
	}
}
