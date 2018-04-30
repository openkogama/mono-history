using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AddDotsToTruncatedText : MonoBehaviour
{
	[SerializeField]
	private Text text;

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		string textValue = text.text;
		bool addDots = false;
		if (text.resizeTextForBestFit)
		{
			text.fontSize = text.cachedTextGenerator.fontSizeUsedForBestFit;
		}
		float res = (float)Screen.width / 1920f;
		float dotSize = (float)CalculateLengthOfMessage("...") / res;
		while (textValue.Length > 0 && (float)CalculateLengthOfMessage(textValue) / res > text.rectTransform.rect.width - dotSize)
		{
			addDots = true;
			textValue = textValue.Remove(Mathf.Max(0, textValue.Length - 1), 1);
			text.text = textValue;
		}
		if (addDots)
		{
			text.text += "...";
		}
		text.fontSize = Mathf.Clamp(text.fontSize, text.resizeTextMinSize, text.resizeTextMaxSize);
	}

	private int CalculateLengthOfMessage(string message)
	{
		float num = 0f;
		float num2 = (float)Screen.width / 1920f;
		CharacterInfo info = default;
		char[] array = message.ToCharArray();
		char[] array2 = array;
		foreach (char ch in array2)
		{
			text.font.GetCharacterInfo(ch, out info, text.cachedTextGenerator.fontSizeUsedForBestFit);
			num += (float)info.advance * num2;
		}
		return (int)num;
	}

	private void OnValidate()
	{
		text = GetComponent<Text>();
	}
}
