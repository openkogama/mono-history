using System;
using UnityEngine;

public class RollingNumberCounter : MonoBehaviour
{
	public int numDigits = 5;

	private RollingNumberDigit[] digits;

	public bool UseOverlay
	{
		set
		{
			for (int i = 0; i < numDigits; i++)
			{
				digits[i].UseOverlay = value;
			}
		}
	}

	public void Initialize()
	{
		digits = new RollingNumberDigit[numDigits];
		Vector3 position = transform.position;
		for (int i = 0; i < numDigits; i++)
		{
			digits[i] = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/RollingNumbers/RollingNumberDigit"), position, Quaternion.identity)).GetComponent<RollingNumberDigit>();
			position += new Vector3(digits[i].DisplayWidth, 0f, 0f);
			digits[i].transform.parent = transform;
		}
	}

	public void SetCounter(int value)
	{
		string text = value.ToString();
		if (text.Length > numDigits)
		{
			Debug.LogError("Too many digits to display in rolling-counter!");
			return;
		}
		if (text.Length < numDigits)
		{
			string text2 = string.Empty;
			for (int i = 0; i < numDigits - text.Length; i++)
			{
				text2 += "0";
			}
			text = text2 + text;
		}
		for (int j = 0; j < numDigits; j++)
		{
			digits[j].Number = Convert.ToInt32(text.Substring(j, 1));
		}
	}
}
