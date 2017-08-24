using UnityEngine;

public class FirstTimeActivatableExitPlayInEditPopup : FirstTimeActivatableMessage
{
	private bool setFirstTimeEventOnEnable;

	protected override void OnDisable()
	{
		base.OnDisable();
		if (!IsEventAllowedInMode)
		{
			Object.Destroy(this);
		}
		else if (firstTimeEventMessage != null)
		{
			setFirstTimeEventOnEnable = true;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (setFirstTimeEventOnEnable)
		{
			DestroyMessage();
			FirstTimeEventManager.SetFirstTimeEvent(firstTimeEvent);
			setFirstTimeEventOnEnable = false;
		}
	}
}
