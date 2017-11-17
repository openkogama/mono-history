using System.Collections;
using MV.Common;
using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class FirstTimeActivatableElementBase : FirstTimeEventHandler, IActivatableFirstTimeUiElement, IFirstTimeEventSkip, IEventSystemHandler
{
	protected bool isRegistered;

	[SerializeField]
	private SoundStyle onShowSound = SoundStyle.FirstTimeElementShown;

	[SerializeField]
	private FirstTimeEvent prerequisiteEvent = FirstTimeEvent.NoEvent;

	[SerializeField]
	private int priority;

	[SerializeField]
	private MVGameMode eventAllowedForMode;

	[SerializeField]
	private bool eventAllowedInAnyMode;

	[SerializeField]
	protected float delayBeforeShown;

	[SerializeField]
	[Tooltip("Set false to avoid checking for blocking elements in the stack.")]
	private bool checkForStackBlocking = true;

	private bool waitingForDelay;

	public FirstTimeEvent FirstTimeEvent => firstTimeEvent;

	public FirstTimeEvent PrerequisiteEvent => prerequisiteEvent;

	public int Priority => priority;

	public bool IsRegistered => isRegistered;

	protected bool IsEventAllowedInMode
	{
		get
		{
			if (eventAllowedInAnyMode)
			{
				return true;
			}
			return eventAllowedForMode == MVGameControllerBase.GameMode;
		}
	}

	public abstract bool CanShow { get; }

	public bool IsShowing { get; private set; }

	protected bool IsBlocked
	{
		get
		{
			if (!checkForStackBlocking)
			{
				return false;
			}
			bool isBlocked = false;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				isBlocked = x.IsUIElementBlocked(gameObject);
			});
			return isBlocked;
		}
	}

	public void Show()
	{
		IsShowing = true;
		OnActivate();
		if (delayBeforeShown > 0f)
		{
			StartCoroutine(ShowDelay());
		}
		else
		{
			DoShow();
		}
	}

	private IEnumerator ShowDelay()
	{
		waitingForDelay = true;
		yield return new WaitForSeconds(delayBeforeShown);
		waitingForDelay = false;
		DoShow();
	}

	private void DoShow()
	{
		OnShow();
		Styles.PlayUISound(onShowSound);
	}

	public virtual void OnShow()
	{
	}

	public virtual void OnActivate()
	{
	}

	protected virtual void OnEnable()
	{
		if (waitingForDelay)
		{
			StartCoroutine(ShowDelay());
		}
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void Start()
	{
		if (IsEventAllowedInMode)
		{
			FirstTimeEventManager.SubscribeToFirstTimeState(OnFirstTimeState);
		}
		else
		{
			Object.Destroy(this);
		}
	}

	protected virtual void OnFirstTimeState(FirstTimeState firstTimeState, FirstTimeEvent latestFirstTimeEvent)
	{
		if (!firstTimeState.HasFirstTimeEventOccured(firstTimeEvent) && !isRegistered)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
			{
				x.RegisterActivatableElement(this);
			});
			isRegistered = true;
		}
		else if (firstTimeState.HasFirstTimeEventOccured(firstTimeEvent))
		{
			UnRegister();
			Object.Destroy(this);
		}
	}

	protected virtual void OnDestroy()
	{
		UnRegister();
		FirstTimeEventManager.UnSubscribeToFirstTimeState(OnFirstTimeState);
	}

	protected void UnRegister()
	{
		if (isRegistered)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
			{
				x.UnRegisterActivatableElement(this);
			});
			isRegistered = false;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
			{
				x.RequestEvaluateActivatableElements();
			});
		}
	}

	public void SkipEvent()
	{
		if (isRegistered)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
			{
				x.SkipFirstTimeEvent(FirstTimeEvent, this);
			});
		}
		else
		{
			Debug.LogError("Trying to skip unregistered event. This shouldn't happen.");
		}
	}
}
