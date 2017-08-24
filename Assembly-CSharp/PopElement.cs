using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopElement : MonoBehaviour
{
	[SerializeField]
	private List<UIGroupFlags> popGroups = new List<UIGroupFlags> { UIGroupFlags.None };

	public void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	[SerializeField]
	public void PopGroups()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			UIGroupFlags uIGroupFlags = UIGroupFlags.None;
			for (int i = 0; i < popGroups.Count; i++)
			{
				uIGroupFlags |= popGroups[i];
			}
			x.PopGroups(uIGroupFlags);
		});
	}
}
