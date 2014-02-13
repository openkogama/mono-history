using System.Collections.Generic;

namespace Localize;

public class TextCollection
{
	private List<string> text = new List<string>();

	public TextCollection(List<string> Text)
	{
		foreach (string item in Text)
		{
			text.Add(item);
		}
	}

	public string GetBaseString(TextSlotIndex textSlotIndex)
	{
		if (text.Count > (int)textSlotIndex)
		{
			return text[(int)textSlotIndex];
		}
		return null;
	}
}
