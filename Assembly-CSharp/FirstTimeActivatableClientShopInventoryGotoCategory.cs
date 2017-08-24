using UnityEngine.EventSystems;

public class FirstTimeActivatableClientShopInventoryGotoCategory : FirstTimeActivatableElementBase
{
	private bool showing;

	public override bool CanShow
	{
		get
		{
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			return !isBlocked && activeInHierarchy;
		}
	}

	public override void OnShow()
	{
		if (!showing)
		{
			DoShowing();
		}
	}

	private void DoShowing()
	{
		showing = true;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (InventoryController x, BaseEventData y) =>
		{
			x.SelectTab(3, 0, 0);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (InventoryController x, BaseEventData y) =>
		{
			x.TabSelected(3);
		});
	}

	protected override void OnDestroy()
	{
		if (isRegistered)
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
			base.OnDestroy();
		}
	}
}
