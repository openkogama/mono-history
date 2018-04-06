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
		yield return new WaitForEndOfFrame();
		string textValue = text.text;
		bool addDots = false;
		text.fontSize = text.cachedTextGenerator.fontSizeUsedForBestFit;
		text.resizeTextForBestFit = false;
		while (textValue.Length > 0 && (float)CalculateLengthOfMessage(textValue) > text.rectTransform.rect.width - 15f)
		{
			addDots = true;
			textValue = textValue.Remove(Mathf.Max(0, textValue.Length - 1), 1);
			text.text = textValue;
		}
		if (addDots)
		{
			text.text += "...";
		}
	}

	private int CalculateLengthOfMessage(string message)
	{
		int num = 0;
		CharacterInfo info = default;
		char[] array = message.ToCharArray();
		char[] array2 = array;
		foreach (char ch in array2)
		{
			text.font.GetCharacterInfo(ch, out info, text.cachedTextGenerator.fontSizeUsedForBestFit);
			num += info.advance;
		}
		return num;
	}

	private void OnValidate()
	{
		text = GetComponent<Text>();
	}
}
