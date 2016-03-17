using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class FullscreenToggleExecute : ToggleHandler
{
	[SerializeField]
	private ToggleStateHandler toggleStateHandler;

	private void Awake()
	{
		FullScreenController.OnFullScreenChange = (UnityAction<bool>)Delegate.Combine(FullScreenController.OnFullScreenChange, new UnityAction<bool>(FullscreenChanged));
	}

	public override void ExecuteToggleState(bool toggleState, UnityAction<bool> toggleCallback)
	{
		if (!FullScreenController.AllowFullscreenChange())
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Fullscreen is not supported in \nInternet Explorer version 8+.\n\nPlease use another browser\n to use fullscreen."), TM._("Error"));
			});
		}
		else
		{
			toggleCallback(!toggleState);
			FullScreenController.FullScreen = toggleState;
		}
	}

	private void FullscreenChanged(bool fullscreenState)
	{
		toggleStateHandler.ToggleState = fullscreenState;
	}
}
