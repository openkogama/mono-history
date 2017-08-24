using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatableMessagePopup : FirstTimeActivatableElementBase
{
	private float showedTime;

	private bool isShown;

	private bool isDone;

	[SerializeField]
	private string messageText;

	[SerializeField]
	private FirstTimeEventMessage firstTimeEventMessagePrefab;

	[SerializeField]
	private List<UIPushOption> pushOptions = new List<UIPushOption>();

	[SerializeField]
	private float stayTimeInSeconds = 2f;

	protected FirstTimeEventMessage firstTimeEventMessage;

	public override bool CanShow => !IsBlocked && gameObject.activeInHierarchy;

	public override void OnShow()
	{
		firstTimeEventMessage = Object.Instantiate(firstTimeEventMessagePrefab);
		firstTimeEventMessage.SetText(TM._(messageText));
		isShown = true;
		PushToStack();
		firstTimeEventMessage.FadeIn();
	}

	private void PushToStack()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			UIPushOption uIPushOption = UIPushOption.None;
			for (int i = 0; i < pushOptions.Count; i++)
			{
				uIPushOption |= pushOptions[i];
			}
			x.Push(firstTimeEventMessage.gameObject, uIPushOption, OnPop, UIGroupFlags.Popup);
		});
	}

	private void OnFinished(GameObject firstTimeEventMessage)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			if (x.PopToStackElement(firstTimeEventMessage))
			{
				x.Pop();
			}
		});
	}

	private void OnPop()
	{
		FirstTimeEventManager.SetFirstTimeEvent(firstTimeEvent);
	}

	public void FadeOut()
	{
		firstTimeEventMessage.FadeOut(OnFinished);
	}

	private void Update()
	{
		if (!isDone)
		{
			if (showedTime > stayTimeInSeconds)
			{
				FadeOut();
				isDone = true;
			}
			if (isShown)
			{
				showedTime += Time.deltaTime;
			}
		}
	}
}
