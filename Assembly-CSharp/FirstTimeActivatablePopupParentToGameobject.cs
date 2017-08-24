using UnityEngine;

public class FirstTimeActivatablePopupParentToGameobject : FirstTimeActivatablePopup
{
	protected override void DoShow()
	{
		CreatePopup();
		FirstTimeChildDestroyedCallback component = popup.GetComponent<FirstTimeChildDestroyedCallback>();
		if (component != null)
		{
			component.SubscribeToOnDestroyed(OnGameObjectDestroyed);
		}
		ParentToTransform();
	}

	protected override void OnDestroy()
	{
		if (isRegistered)
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		}
		if (popup != null)
		{
			Object.Destroy(popup.gameObject);
		}
		base.OnDestroy();
	}

	private void OnGameObjectDestroyed()
	{
		if (isRegistered)
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		}
		Object.Destroy(this);
	}

	private void ParentToTransform()
	{
		popup.transform.SetParent(transform, worldPositionStays: false);
	}
}
