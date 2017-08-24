using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatablePointerWaitForCubeTool : FirstTimeActivatableElementBase
{
	[SerializeField]
	private bool checkCubeCount;

	[SerializeField]
	private List<EditCubeChange> cubeChangesToCheck;

	[SerializeField]
	private int numberOfChangesBeforePointer;

	[SerializeField]
	private FirstTimeEventPopupWithProgress meanwhilePopup;

	private FirstTimeEventPopupWithProgress popup;

	private int currentChangeCount;

	private bool isShown;

	private bool canShow = true;

	private bool completed;

	[SerializeField]
	protected bool skipAllowed = true;

	public override bool CanShow => !IsBlocked && gameObject.activeInHierarchy && canShow;

	public override void OnActivate()
	{
		CubeModelTool.OnEditCubeChange = (Action<int, EditCubeChange>)Delegate.Combine(CubeModelTool.OnEditCubeChange, new Action<int, EditCubeChange>(OnCubeChanged));
	}

	public override void OnShow()
	{
		if (isShown)
		{
			Debug.LogError("Shown called more than once");
			return;
		}
		isShown = true;
		popup = UnityEngine.Object.Instantiate(meanwhilePopup);
		popup.SetSkippable(skipAllowed);
		popup.FadeIn();
		popup.SetProgress(currentChangeCount, numberOfChangesBeforePointer);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.None, OnClosed, UIGroupFlags.Popup);
		});
	}

	private void OnCubeChanged(int cubeCount, EditCubeChange changeMade)
	{
		if (checkCubeCount)
		{
			currentChangeCount = cubeCount;
		}
		else
		{
			for (int i = 0; i < cubeChangesToCheck.Count; i++)
			{
				if (cubeChangesToCheck[i] == changeMade)
				{
					currentChangeCount++;
					break;
				}
			}
		}
		if (isShown)
		{
			popup.SetProgress(currentChangeCount, numberOfChangesBeforePointer);
			if (currentChangeCount >= numberOfChangesBeforePointer)
			{
				OnShown();
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CubeModelTool.OnEditCubeChange = (Action<int, EditCubeChange>)Delegate.Remove(CubeModelTool.OnEditCubeChange, new Action<int, EditCubeChange>(OnCubeChanged));
	}

	private void OnClosed()
	{
		CubeModelTool.OnEditCubeChange = (Action<int, EditCubeChange>)Delegate.Remove(CubeModelTool.OnEditCubeChange, new Action<int, EditCubeChange>(OnCubeChanged));
		if (!completed)
		{
			popup.StartFade(OnPopupRemoved);
		}
	}

	protected void OnShown()
	{
		completed = true;
		canShow = false;
		CubeModelTool.OnEditCubeChange = (Action<int, EditCubeChange>)Delegate.Remove(CubeModelTool.OnEditCubeChange, new Action<int, EditCubeChange>(OnCubeChanged));
		popup.StartFade(OnPopupRemoved);
	}

	private void OnPopupRemoved(GameObject popupGameObject)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			if (x.PopToStackElement(popupGameObject))
			{
				x.Pop();
			}
		});
		UnityEngine.Object.Destroy(this);
		if (completed)
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		}
		else
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		}
	}
}
