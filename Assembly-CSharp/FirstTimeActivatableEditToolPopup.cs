using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatableEditToolPopup : FirstTimeActivatableElementBase
{
	[SerializeField]
	private List<EditCubeChange> cubeChangesToCheck;

	[SerializeField]
	private FirstTimeEventPopup popupPrefab;

	private FirstTimeEventPopup popup;

	private bool showing;

	[SerializeField]
	protected bool skipAllowed = true;

	public override bool CanShow => !IsBlocked && gameObject.activeInHierarchy;

	public override void OnShow()
	{
		if (!showing)
		{
			CubeModelTool.OnEditCubeChange = (Action<int, EditCubeChange>)Delegate.Combine(CubeModelTool.OnEditCubeChange, new Action<int, EditCubeChange>(OnCubeChanged));
			popup = UnityEngine.Object.Instantiate(popupPrefab);
			popup.SetSkippable(skipAllowed);
			popup.FadeIn();
			showing = true;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.None, null, UIGroupFlags.Popup);
			});
		}
	}

	private void OnCubeChanged(int cubeCount, EditCubeChange changeMade)
	{
		for (int i = 0; i < cubeChangesToCheck.Count; i++)
		{
			if (cubeChangesToCheck[i] == changeMade)
			{
				OnShown();
				break;
			}
		}
	}

	protected void OnShown()
	{
		CubeModelTool.OnEditCubeChange = (Action<int, EditCubeChange>)Delegate.Remove(CubeModelTool.OnEditCubeChange, new Action<int, EditCubeChange>(OnCubeChanged));
		if (popup != null)
		{
			popup.StartFade(OnPopupRemoved);
		}
	}

	private void OnPopupRemoved(GameObject popupGameObject)
	{
		Debug.Log(string.Concat("OnShown ", FirstTimeEvent, "  ", Time.frameCount));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			if (x.PopToStackElement(popupGameObject))
			{
				x.Pop();
			}
		});
		UnityEngine.Object.Destroy(this);
		FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
	}
}
