using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FirstTimeButtonClicked : FirstTimeEventHandler
{
	private bool isReady;

	private bool buttonHasBeenClicked;

	[SerializeField]
	private Button button;

	[SerializeField]
	private FirstTimeActivatableElementBase firstTimeActivatableElementBase;

	private void Start()
	{
		button.onClick.AddListener(Clicked);
		FirstTimeEventManager.SubscribeToFirstTimeState(FirstTimeStateReceiver);
	}

	private void FirstTimeStateReceiver(FirstTimeState firstTimeState, FirstTimeEvent latestFirstTimeEvent)
	{
		if (FirstTimeEventManager.HasFirstTimeEventOccured(firstTimeEvent))
		{
			Object.Destroy(this);
			return;
		}
		isReady = true;
		if (buttonHasBeenClicked)
		{
			HandleFirstTimeEvent();
		}
	}

	private void Clicked()
	{
		buttonHasBeenClicked = true;
		if (isReady)
		{
			HandleFirstTimeEvent();
		}
	}

	private void HandleFirstTimeEvent()
	{
		if (firstTimeActivatableElementBase != null && !FirstTimeEventManager.HasFirstTimeEventOccured(firstTimeActivatableElementBase.FirstTimeEvent))
		{
			FirstTimeEventManager.SetFirstTimeEvent(firstTimeActivatableElementBase.FirstTimeEvent);
		}
		FirstTimeEventManager.SetFirstTimeEvent(firstTimeEvent);
		Object.Destroy(this);
	}

	private void OnDestroy()
	{
		FirstTimeEventManager.UnSubscribeToFirstTimeState(FirstTimeStateReceiver);
		button.onClick.RemoveListener(Clicked);
	}
}
