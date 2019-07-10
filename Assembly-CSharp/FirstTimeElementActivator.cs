using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeElementActivator : MonoBehaviour, IFirstTimeElementActivator, IEventSystemHandler
{
	private bool evaluateActivatableElements;

	private bool isReady;

	[SerializeField]
	private UIStack uiStack;

	[SerializeField]
	private FirstTimeEventSkipPopup firstTimeSkipPopup;

	private Dictionary<FirstTimeEvent, IActivatableFirstTimeUiElement> activatableUiElements = new Dictionary<FirstTimeEvent, IActivatableFirstTimeUiElement>();

	private List<FirstTimeEvent> elementsToRemove = new List<FirstTimeEvent>();

	protected void Start()
	{
		MVGameControllerBase.OnJoinStateChanged = (Action<MVJoinState>)Delegate.Combine(MVGameControllerBase.OnJoinStateChanged, new Action<MVJoinState>(OnJoinStateChanged));
		uiStack.SubscribeToStackChanges(OnStackChange);
		FirstTimeEventManager.SubscribeToFirstTimeState(FirstTimeStateReceiver);
		evaluateActivatableElements = true;
		FirstTimeEventManager.XPRewarded = (Action)Delegate.Combine(FirstTimeEventManager.XPRewarded, new Action(OnXPRewarded));
	}

	protected void OnDestroy()
	{
		FirstTimeEventManager.XPRewarded = (Action)Delegate.Remove(FirstTimeEventManager.XPRewarded, new Action(OnXPRewarded));
		FirstTimeEventManager.UnSubscribeToFirstTimeState(FirstTimeStateReceiver);
		uiStack.UnSubscribeToStackChanges(OnStackChange);
	}

	private void OnJoinStateChanged(MVJoinState mvJoinState)
	{
		if (mvJoinState == MVJoinState.Playing)
		{
			isReady = true;
			MVGameControllerBase.OnJoinStateChanged = (Action<MVJoinState>)Delegate.Remove(MVGameControllerBase.OnJoinStateChanged, new Action<MVJoinState>(OnJoinStateChanged));
		}
	}

	private void FirstTimeStateReceiver(FirstTimeState firstTimeState, FirstTimeEvent firstTimeEvent)
	{
		evaluateActivatableElements = true;
	}

	private void OnStackChange()
	{
		evaluateActivatableElements = true;
	}

	private void OnXPRewarded()
	{
		if (MVGameControllerBase.GameMode != MVGameMode.Play)
		{
			NotificationController.PushNoticationInstruction("Great job! XP rewarded!", NotificationLifetime.Low);
		}
	}

	private void LateUpdate()
	{
		if (isReady && evaluateActivatableElements)
		{
			evaluateActivatableElements = false;
			EvaluateActivatableElements();
		}
	}

	private void EvaluateActivatableElements()
	{
		IActivatableFirstTimeUiElement activatableFirstTimeUiElement = null;
		foreach (KeyValuePair<FirstTimeEvent, IActivatableFirstTimeUiElement> activatableUiElement in activatableUiElements)
		{
			if (!activatableUiElement.Value.IsRegistered)
			{
				Debug.LogWarning("In rare cases when entering play mode while in on boarding flow the unregister event does not reach the activator because of the entire hierarchy being disabled. This hack handles this case.");
				elementsToRemove.Add(activatableUiElement.Key);
			}
			else if (activatableUiElement.Value.CanShow && FirstTimeEventManager.HasFirstTimeEventOccured(activatableUiElement.Value.PrerequisiteEvent))
			{
				if (activatableFirstTimeUiElement == null)
				{
					activatableFirstTimeUiElement = activatableUiElement.Value;
				}
				else if (activatableUiElement.Value.Priority > activatableFirstTimeUiElement.Priority)
				{
					activatableFirstTimeUiElement = activatableUiElement.Value;
				}
			}
		}
		if (activatableFirstTimeUiElement != null && !activatableFirstTimeUiElement.IsShowing)
		{
			activatableFirstTimeUiElement.Show();
			MVGameControllerBase.GameEventManager.NotifyFirstTimeEvent(activatableFirstTimeUiElement.FirstTimeEvent);
		}
		foreach (FirstTimeEvent item in elementsToRemove)
		{
			activatableUiElements.Remove(item);
		}
		elementsToRemove.Clear();
	}

	public void RegisterActivatableElement(IActivatableFirstTimeUiElement firstTimeEventHandlerListener)
	{
		if (!activatableUiElements.ContainsKey(firstTimeEventHandlerListener.FirstTimeEvent))
		{
			activatableUiElements.Add(firstTimeEventHandlerListener.FirstTimeEvent, firstTimeEventHandlerListener);
		}
	}

	public void SkipFirstTimeEvent(FirstTimeEvent firstTimeEvent, FirstTimeActivatableElementBase firstTimeActivatable)
	{
		if (FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.SkipEvent))
		{
			ExecuteEvents.ExecuteHierarchy(uiStack.gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Popup);
			});
			FirstTimeEventManager.SetFirstTimeEvent(firstTimeEvent);
			UnityEngine.Object.Destroy(firstTimeActivatable);
		}
		else
		{
			FirstTimeEventSkipPopup popup = UnityEngine.Object.Instantiate(firstTimeSkipPopup);
			popup.Initialize(firstTimeEvent, firstTimeActivatable);
			ExecuteEvents.ExecuteHierarchy(uiStack.gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
		}
	}

	public void UnRegisterActivatableElement(IActivatableFirstTimeUiElement firstTimeEventHandlerListener)
	{
		activatableUiElements.Remove(firstTimeEventHandlerListener.FirstTimeEvent);
	}

	public void RequestEvaluateActivatableElements()
	{
		evaluateActivatableElements = true;
	}
}
