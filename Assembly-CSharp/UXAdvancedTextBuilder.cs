using System.Collections.Generic;
using UnityEngine;

public class UXAdvancedTextBuilder : MonoBehaviour
{
	private UXText textPrefab;

	private AdvancedTextData lastAdded;

	private List<AdvancedTextData> buildTexts = new List<AdvancedTextData>();

	private List<UXText> cachedTexts = new List<UXText>();

	public bool ignoreClipping;

	public void ResetBuild()
	{
		buildTexts.Clear();
		lastAdded = new AdvancedTextData
		{
			textData = string.Empty,
			colorData = Color.white
		};
	}

	public void AddText(string text)
	{
		AddText(text, Color.white);
	}

	public void AddText(string text, Color color, float zOffset = 0f)
	{
		if (color == lastAdded.colorData)
		{
			buildTexts.Remove(lastAdded);
			text = lastAdded.textData + text;
		}
		AdvancedTextData item = new AdvancedTextData
		{
			textData = text,
			colorData = color,
			offset = zOffset
		};
		buildTexts.Add(item);
		lastAdded = item;
	}

	public List<UXText> BuildTexts(Transform root)
	{
		Debug.Log("Build texts");
		if (textPrefab == null)
		{
			textPrefab = Resources.Load("Prefabs/UX/Text", typeof(UXText)) as UXText;
		}
		List<UXText> list = new List<UXText>();
		float num = 0f;
		while (cachedTexts.Count < buildTexts.Count)
		{
			UXText uXText = Object.Instantiate(textPrefab);
			uXText.SetAlignment(UXHorizontal.Left, UXVertical.Top);
			uXText.WordWrap = true;
			uXText.wrapStyle = TextWrapStyle.Mixed;
			uXText.wordWrapWidth = 10f;
			cachedTexts.Add(uXText);
		}
		int num2 = 0;
		foreach (AdvancedTextData buildText in buildTexts)
		{
			UXText uXText2 = cachedTexts[num2];
			uXText2.Text = buildText.textData;
			uXText2.Color = buildText.colorData;
			uXText2.ignoreClipping = ignoreClipping;
			float textWidth = uXText2.TextWidth;
			Debug.Log("TextWidht " + textWidth);
			uXText2.transform.parent = root;
			uXText2.transform.localScale = Vector2.one;
			uXText2.transform.localPosition = new Vector3(num, 0f, 0.1f + buildText.offset);
			list.Add(uXText2);
			num += textWidth;
			num2++;
		}
		ResetBuild();
		return list;
	}
}
