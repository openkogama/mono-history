using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UXTextSizeCalculator : UXViewScript
{
	public UXText textObject;

	private bool initialized;

	private char[] chars = new char[66]
	{
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
		'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
		'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
		'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
		'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
		'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7',
		'8', '9', ' ', '.', ',', ':'
	};

	private Dictionary<UXTextSize, Dictionary<char, Vector2>> char2size = new Dictionary<UXTextSize, Dictionary<char, Vector2>>();

	public bool IsInitialized => initialized;

	public Vector2 MeasureString(string text, UXTextSize textSize = UXTextSize.Medium)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return MeasureString(text, Vector2.one, textSize);
	}

	public Vector2 MeasureString(string text, Vector2 scale, UXTextSize textSize)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			return Vector2.zero;
		}
		string[] array = text.Split(new char[1] { '\n' });
		float num = 0f;
		float num2 = 0f;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			float num3 = 0f;
			float num4 = 0f;
			if (text2 == string.Empty)
			{
				if (!char2size[textSize].ContainsKey('A'))
				{
					MeasureCharSize('A', textSize);
				}
				num2 += char2size[textSize]['A'].y;
			}
			foreach (char c in text2)
			{
				if (!char2size[textSize].ContainsKey(c))
				{
					MeasureCharSize(c, textSize);
				}
				Vector2 val = char2size[textSize][c];
				num3 += val.x;
				num4 = Mathf.Max(num4, val.y);
			}
			num = Mathf.Max(num, num3);
			num2 += num4;
		}
		return new Vector2(num * scale.x, num2 * scale.y);
	}

	private void MeasureCharSize(char c, UXTextSize textSize)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (c.Equals(' '))
		{
			SpecialSpaceCase(textSize);
			return;
		}
		textObject.TextSize = textSize;
		textObject.Text = string.Empty + c;
		Vector2 value = textObject.RenderAndGetTextBounds();
		char2size[textSize].Add(c, value);
		textObject.Text = string.Empty;
	}

	private void SpecialSpaceCase(UXTextSize textSize)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		textObject.TextSize = textSize;
		textObject.Text = " A";
		Vector2 val = textObject.RenderAndGetTextBounds();
		char2size[textSize].Add(' ', new Vector2(val.x - char2size[textSize]['A'].x, val.y));
		textObject.Text = string.Empty;
	}

	private IEnumerator MeasureBasicChars()
	{
		yield return (object)new WaitForEndOfFrame();
		initialized = true;
		foreach (int textSize in Enum.GetValues(typeof(UXTextSize)))
		{
			char2size.Add((UXTextSize)textSize, new Dictionary<char, Vector2>());
			char[] array = chars;
			foreach (char c in array)
			{
				if (!char2size[(UXTextSize)textSize].ContainsKey(c))
				{
					MeasureCharSize(c, (UXTextSize)textSize);
				}
			}
		}
	}

	public override void OnInitialize()
	{
		((MonoBehaviour)this).StartCoroutine(MeasureBasicChars());
	}
}
