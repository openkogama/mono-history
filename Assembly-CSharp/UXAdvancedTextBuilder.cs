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
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		buildTexts.Clear();
		lastAdded = new AdvancedTextData
		{
			textData = string.Empty,
			colorData = Color.white
		};
	}

	public void AddText(string text)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		AddText(text, Color.white);
	}

	public void AddText(string text, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (color == lastAdded.colorData)
		{
			buildTexts.Remove(lastAdded);
			text = lastAdded.textData + text;
		}
		AdvancedTextData item = new AdvancedTextData
		{
			textData = text,
			colorData = color
		};
		buildTexts.Add(item);
		lastAdded = item;
	}

	public List<UXText> BuildTexts(Transform root)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)textPrefab == (Object)null)
		{
			textPrefab = Resources.Load("Prefabs/UX/Text", typeof(UXText)) as UXText;
		}
		List<UXText> list = new List<UXText>();
		float num = 0f;
		while (cachedTexts.Count < buildTexts.Count)
		{
			cachedTexts.Add(Object.Instantiate((Object)(object)textPrefab) as UXText);
		}
		int num2 = 0;
		foreach (AdvancedTextData buildText in buildTexts)
		{
			UXText uXText = cachedTexts[num2];
			uXText.Text = buildText.textData;
			uXText.Color = buildText.colorData;
			uXText.ignoreClipping = ignoreClipping;
			float textWidth = uXText.TextWidth;
			((Component)uXText).transform.parent = root;
			((Component)uXText).transform.localScale = Vector2.op_Implicit(Vector2.one);
			((Component)uXText).transform.localPosition = new Vector3(num + textWidth / 2f, 0f, 0.1f);
			list.Add(uXText);
			num += textWidth;
			num2++;
		}
		ResetBuild();
		return list;
	}
}
