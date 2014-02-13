using System.Collections.Generic;
using UnityEngine;

public class ScrollableBoxData : DialogData
{
	public List<UXLine> lines;

	public override void ApplyDataToElement(GameObject element)
	{
		UXScrollableBox component = element.GetComponent<UXScrollableBox>();
		if ((Object)(object)component == (Object)null)
		{
			return;
		}
		foreach (UXLine line in lines)
		{
			component.AddLine(line);
		}
	}
}
