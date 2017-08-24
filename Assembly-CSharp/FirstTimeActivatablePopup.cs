using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatablePopup : FirstTimeActivatableElementBase
{
	[SerializeField]
	private FirstTimeEventPopup popupPrefab;

	[SerializeField]
	private List<UIPushOption> pushOptions = new List<UIPushOption>();

	[SerializeField]
	protected bool skipAllowed = true;

	protected FirstTimeEventPopup popup;

	public override bool CanShow => !IsBlocked && gameObject.activeInHierarchy;

	public override void OnShow()
	{
		if (!(popup != null))
		{
			DoShow();
		}
	}

	protected virtual void DoShow()
	{
		CreatePopup();
		PushToStack();
	}

	protected void CreatePopup()
	{
		popup = Object.Instantiate(popupPrefab);
		popup.SetSkippable(skipAllowed);
		popup.FadeIn();
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
			x.Push(popup.gameObject, uIPushOption, OnPop, UIGroupFlags.Popup);
		});
	}

	private void OnPop()
	{
		if (isRegistered)
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
			Object.Destroy(this);
		}
	}
}
