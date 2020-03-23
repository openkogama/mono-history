using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIStack : MonoBehaviour, IUIStack, IEventSystemHandler
{
	private class StackElement
	{
		public readonly GameObject gameObject;

		public readonly bool blockingObject;

		public readonly bool hideAll;

		public readonly bool invisibleBlocker;

		public readonly bool hideAllExceptStackbottom;

		public readonly bool suppressInput;

		public readonly UnityAction onPop;

		public readonly UIGroupFlags group;

		public readonly string name;

		public StackElement(GameObject gameObject, UIPushOption pushOption, UnityAction onPop, UIGroupFlags group)
		{
			name = gameObject.name;
			this.gameObject = gameObject;
			blockingObject = (pushOption & UIPushOption.Blocking) != 0 || (pushOption & UIPushOption.InvisibleBlocker) != 0;
			hideAll = (pushOption & UIPushOption.HideAll) != 0;
			invisibleBlocker = (pushOption & UIPushOption.InvisibleBlocker) != 0;
			hideAllExceptStackbottom = (pushOption & UIPushOption.HideAllExceptStackBottom) != 0;
			suppressInput = (pushOption & UIPushOption.SuppressInput) != 0;
			this.onPop = onPop;
			this.group = group;
		}
	}

	private Action uiStackChangedPublisher;

	[SerializeField]
	private GameObject root;

	[SerializeField]
	private GameObject blockingObject;

	[SerializeField]
	private DisableInput inputBlocker;

	[SerializeField]
	private Image blockingObjectImage;

	private float origBlockerAlpha = 0.5f;

	private bool stackReady;

	private List<StackElement> stackableUiElements = new List<StackElement>();

	public bool StackReady => stackReady;

	private void Start()
	{
		origBlockerAlpha = blockingObjectImage.color.a;
	}

	private void LateUpdate()
	{
		foreach (StackElement stackableUiElement in stackableUiElements)
		{
			if (stackableUiElement.blockingObject)
			{
				MVInputWrapper.SuppressShortcutKeys();
				break;
			}
		}
	}

	public void SubscribeToStackChanges(Action onStackChanged)
	{
		uiStackChangedPublisher = (Action)Delegate.Combine(uiStackChangedPublisher, onStackChanged);
	}

	public void UnSubscribeToStackChanges(Action onStackChanged)
	{
		uiStackChangedPublisher = (Action)Delegate.Remove(uiStackChangedPublisher, onStackChanged);
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
			HideAll();
		}
		if (stackElement.hideAllExceptStackbottom)
		{
			HideAllExceptStackBottom();
		}
		if (stackElement.blockingObject)
		{
			blockingObject.transform.SetAsLastSibling();
			inputBlocker.enabled = stackElement.suppressInput;
			blockingObject.gameObject.SetActive(value: true);
		}
		stackElement.gameObject.transform.SetParent(root.transform, worldPositionStays: false);
		stackElement.gameObject.SetActive(value: true);
		stackableUiElements.Add(stackElement);
		if (!stackReady)
		{
			HideAll();
		}
		UpdateBlocking();
		if (uiStackChangedPublisher != null)
		{
			uiStackChangedPublisher();
		}
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

	public bool PopToStackElement(GameObject gameObject)
	{
		bool flag = false;
		foreach (StackElement stackableUiElement in stackableUiElements)
		{
			if (stackableUiElement.gameObject == gameObject)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Debug.Log(StackTraceUtility.ExtractStackTrace());
			Debug.LogError("PopToStackElement: Element not found abouting");
			return false;
		}
		while (Peak().gameObject != gameObject)
		{
			Pop();
		}
		return true;
	}

	public void SetStackReady()
	{
		stackReady = true;
		UpdateStack();
	}

	public void PopToGroup(UIGroupFlags group)
	{
		bool flag = false;
		foreach (StackElement stackableUiElement in stackableUiElements)
		{
			if (stackableUiElement.group == group)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Debug.Log(StackTraceUtility.ExtractStackTrace());
			Debug.LogError("PopToGroup: Element not found abouting");
		}
		else
		{
			while (stackableUiElements[stackableUiElements.Count - 1].group != group)
			{
				Pop();
			}
		}
	}

	public bool IsUIElementBlocked(GameObject uiElement)
	{
		int index = -1;
		FindStackParent(uiElement.transform, ref index);
		if (index == -1)
		{
			Debug.LogError(uiElement.name + "IsUIElementBlocked index == " + index);
			return false;
		}
		index++;
		if (stackableUiElements.Count <= index)
		{
			return false;
		}
		for (int i = index; i < stackableUiElements.Count; i++)
		{
			if (stackableUiElements[i].blockingObject)
			{
				return true;
			}
		}
		return false;
	}

	public GameObject Peak()
	{
		return stackableUiElements[stackableUiElements.Count - 1].gameObject;
	}

	public bool IsStackEmpty()
	{
		return stackableUiElements.Count <= 2;
	}

	private void HideAll()
	{
		foreach (StackElement stackableUiElement in stackableUiElements)
		{
			stackableUiElement.gameObject.SetActive(value: false);
		}
	}

	private void HideAllExceptStackBottom()
	{
		foreach (StackElement stackableUiElement in stackableUiElements)
		{
			if (stackableUiElement.group != UIGroupFlags.StackBottom)
			{
				stackableUiElement.gameObject.SetActive(value: false);
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
			if (uiStackChangedPublisher != null)
			{
				uiStackChangedPublisher();
			}
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
		UnityEngine.Object.Destroy(stackElement.gameObject);
	}

	private void FindStackParent(Transform uiElement, ref int index)
	{
		for (int i = 0; i < stackableUiElements.Count; i++)
		{
			if (stackableUiElements[i].gameObject.transform == uiElement)
			{
				index = i;
				return;
			}
		}
		FindStackParent(uiElement.transform.parent, ref index);
	}

	private void SetStackVisible()
	{
		if (stackableUiElements.Count >= 2)
		{
			int num = stackableUiElements.Count - 2;
			while (num >= 0 && !stackableUiElements[num + 1].hideAll && !stackableUiElements[num + 1].hideAllExceptStackbottom)
			{
				stackableUiElements[num].gameObject.SetActive(value: true);
				num--;
			}
		}
	}
}
