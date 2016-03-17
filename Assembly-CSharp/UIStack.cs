using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIStack : MonoBehaviour, IEventSystemHandler, IUIStack
{
	private class StackElement
	{
		public readonly GameObject gameObject;

		public readonly bool blockingObject;

		public readonly bool hideAll;

		public readonly UnityAction onPop;

		public readonly UIGroupFlags group;

		public readonly string name;

		public StackElement(GameObject gameObject, UIPushOption pushOption, UnityAction onPop, UIGroupFlags group)
		{
			name = gameObject.name;
			this.gameObject = gameObject;
			blockingObject = (pushOption & UIPushOption.Blocking) != 0;
			hideAll = (pushOption & UIPushOption.HideAll) != 0;
			this.onPop = onPop;
			this.group = group;
		}
	}

	[SerializeField]
	private GameObject root;

	[SerializeField]
	private GameObject blockingObject;

	private List<StackElement> stackableUiElements = new List<StackElement>();

	public void Push(GameObject gameObject, UIPushOption pushOption = UIPushOption.None, UnityAction onPop = null, UIGroupFlags group = UIGroupFlags.Default)
	{
		if (group == UIGroupFlags.None)
		{
			Debug.LogError("Group cannot be none as this makes it impossible to pop");
			return;
		}
		if (Application.isEditor)
		{
			Debug.Log("Push " + gameObject.name);
			UIStackDebugElement uIStackDebugElement = gameObject.AddComponent<UIStackDebugElement>();
			uIStackDebugElement.Initialize(pushOption, group);
		}
		for (int i = 0; i < stackableUiElements.Count; i++)
		{
			if (stackableUiElements[i].gameObject.GetInstanceID() == gameObject.GetInstanceID())
			{
				Debug.LogError("You cannot push an object to the stack twice.");
				return;
			}
		}
		StackElement stackElement = new StackElement(gameObject, pushOption, onPop, group);
		if (stackElement.hideAll)
		{
			foreach (StackElement stackableUiElement in stackableUiElements)
			{
				Debug.Log(stackableUiElement.gameObject.name);
				stackableUiElement.gameObject.SetActive(value: false);
			}
		}
		if (stackElement.blockingObject)
		{
			blockingObject.transform.SetAsLastSibling();
			blockingObject.gameObject.SetActive(value: true);
		}
		stackElement.gameObject.transform.SetParent(root.transform, worldPositionStays: false);
		stackElement.gameObject.SetActive(value: true);
		stackableUiElements.Add(stackElement);
	}

	public void Pop()
	{
		RemoveElement(stackableUiElements.Count - 1);
		UpdateStack();
	}

	public void PopGroups(UIGroupFlags popGroups)
	{
		for (int num = stackableUiElements.Count; num > 0; num--)
		{
			if ((stackableUiElements[num - 1].group & popGroups) > UIGroupFlags.None)
			{
				RemoveElement(num - 1);
			}
		}
		UpdateStack();
	}

	private void LateUpdate()
	{
		foreach (StackElement stackableUiElement in stackableUiElements)
		{
			if (stackableUiElement.blockingObject)
			{
				MVInputWrapper.IsShortcutKeysSuppressed = true;
				break;
			}
		}
	}

	private void UpdateStack()
	{
		if (stackableUiElements.Count != 0)
		{
			StackElement stackElement = stackableUiElements[stackableUiElements.Count - 1];
			Debug.Log("UpdateStack " + stackElement.name);
			stackElement.gameObject.SetActive(value: true);
			SetStackVisible();
			UpdateBlocking();
		}
	}

	private void UpdateBlocking()
	{
		for (int num = stackableUiElements.Count - 1; num >= 0; num--)
		{
			if (stackableUiElements[num].blockingObject)
			{
				blockingObject.transform.SetAsLastSibling();
				int siblingIndex = stackableUiElements[num].gameObject.transform.GetSiblingIndex();
				blockingObject.transform.SetSiblingIndex(siblingIndex);
				blockingObject.gameObject.SetActive(value: true);
				return;
			}
		}
		blockingObject.transform.SetAsFirstSibling();
		blockingObject.gameObject.SetActive(value: false);
	}

	private void RemoveElement(int index)
	{
		StackElement stackElement = stackableUiElements[index];
		stackableUiElements.RemoveAt(index);
		if (stackElement.onPop != null)
		{
			stackElement.onPop();
		}
		Object.Destroy(stackElement.gameObject);
	}

	public void PopToBottom()
	{
		while (stackableUiElements.Count > 1)
		{
			Pop();
		}
	}

	public bool IsStackEmpty()
	{
		return stackableUiElements.Count <= 1;
	}

	private void SetStackVisible()
	{
		if (stackableUiElements.Count >= 2)
		{
			int num = stackableUiElements.Count - 2;
			while (num >= 0 && !stackableUiElements[num + 1].hideAll)
			{
				stackableUiElements[num].gameObject.SetActive(value: true);
				num--;
			}
		}
	}
}
