using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatableExitPlayInEditPointer : FirstTimeActivatableButtonPointer
{
	[SerializeField]
	private GameObject leavePlayInEdit;

	private float timeBeforeActive = 0.3f;

	private float currentTime;

	private bool canShow;

	private bool isDeleting;

	public override bool CanShow
	{
		get
		{
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = leavePlayInEdit.activeInHierarchy;
			return !isBlocked && activeInHierarchy && canShow;
		}
	}

	public override void OnShow()
	{
		ShowBubble();
		isDeleting = false;
	}

	private void Update()
	{
		if (isDeleting)
		{
			ExecuteEvents.ExecuteHierarchy(((DesktopPlayModeController)MVGameControllerBase.IPlayModeUI).gameObject, null, (TextBubbleController x, BaseEventData y) =>
			{
				x.ClearBubblesWithId(bubbleId);
			});
			isDeleting = false;
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
			Object.Destroy(this);
			return;
		}
		currentTime += Time.deltaTime;
		if (currentTime >= timeBeforeActive && !canShow)
		{
			canShow = true;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
			{
				x.RequestEvaluateActivatableElements();
			});
		}
	}

	protected override void OnShown()
	{
		if (canShow)
		{
			isDeleting = true;
			canShow = false;
			ExecuteEvents.ExecuteHierarchy(((DesktopPlayModeController)MVGameControllerBase.IPlayModeUI).gameObject, null, (TextBubbleController x, BaseEventData y) =>
			{
				x.ClearBubblesWithId(bubbleId);
			});
		}
	}
}
