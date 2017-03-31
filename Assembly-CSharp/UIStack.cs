using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIStack : MonoBehaviour, IEventSystemHandler, IUIStack
{
	private class StackElement
	{
		public readonly GameObject gameObject;

		public readonly bool blockingObject;

		public readonly bool hideAll;

		public readonly bool invisibleBlocker;

		public readonly UnityAction onPop;

		public readonly UIGroupFlags group;

		public readonly string name;

		public StackElement(GameObject gameObject, UIPushOption pushOption, UnityAction onPop, UIGroupFlags group)
		{
			name = gameObject.name;
			this.gameObject = gameObject;
			blockingObject = (pushOption & UIPushOption.Blocking) != 0;
			hideAll = (pushOption & UIPushOption.HideAll) != 0;
			invisibleBlocker = (pushOption & UIPushOption.InvisibleBlocker) != 0;
			this.onPop = onPop;
			this.group = group;
		}
	}

	[SerializeField]
	private GameObject root;

	[SerializeField]
	private GameObject blockingObject;

	[SerializeField]
	private Image blockingObjectImage;

	private float origBlockerAlpha = 0.5f;

	private List<StackElement> stackableUiElements = new List<StackElement>();

	private void Start()
	{
		origBlockerAlpha = blockingObjectImage.color.a;
	}

	public void Push(GameObject gameObject, UIPushOption pushOption = UIPushOption.None, UnityAction onPop = null, UIGroupFlags group = UIGroupFlags.Default)
	{
		if (group == UIGroupFlags.None)
		{
			Debug.LogError("Group cannot be none as this makes it impossible to pop");
			return;
		}
		if (Application.isEditor)
		{
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
		UpdateBlocking();
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
				if (stackableUiElements[num].invisibleBlocker)
				{
					Color color = blockingObjectImage.color;
					color.a = 0f;
					blockingObjectImage.color = color;
				}
				else
				{
					Color color2 = blockingObjectImage.color;
					color2.a = origBlockerAlpha;
					blockingObjectImage.color = color2;
				}
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
