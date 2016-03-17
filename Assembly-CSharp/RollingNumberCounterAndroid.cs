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
			Debug.LogError("Too many digits to display in rolling-counter!");
			return;
		}
		if (text.Length < num)
		{
			string text2 = string.Empty;
			for (int i = 0; i < num - text.Length; i++)
			{
				text2 += "0";
			}
			text = text2 + text;
		}
		for (int j = 0; j < num; j++)
		{
			digits[j].Number = Convert.ToInt32(text.Substring(j, 1));
		}
	}
}
