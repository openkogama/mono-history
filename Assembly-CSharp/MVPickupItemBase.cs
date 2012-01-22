using MV.WorldObject;
using UnityEngine;

public abstract class MVPickupItemBase : MVLogicObject
{
	private TriggerBoxEvents triggerBoxEvents;

	protected override void CreateMVWOC(bool local)
	{
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if ((Object)(object)triggerBoxEvents != (Object)null)
		{
			triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
			triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		}
		else
		{
			Debug.LogError((object)("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name));
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.AvatarState == AvatarState.Playing)
		{
			MVGameController.Instance.Game.TriggerBoxEnter(this);
		}
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		if (MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.AvatarState == AvatarState.Playing)
		{
			MVGameController.Instance.Game.TriggerBoxExit(this);
		}
	}

	public void HandleStateChange(PickupItemState state, int instigatorActorNr)
	{
		Debug.Log((object)"Base HandleStateChange");
		switch (state)
		{
		case PickupItemState.Listening:
			OnRespawn();
			break;
		case PickupItemState.Pickup:
			if (instigatorActorNr == MVGameController.Instance.WOCM.LocalPlayerActorNumber)
			{
				MVGameController.Instance.WOCM.LocalPlayer.Avatar.HandlePickupItem(WorldObjectType, instigatorActorNr);
			}
			OnPickup();
			break;
		case PickupItemState.Counting:
			break;
		}
	}

	public virtual void OnPickup()
	{
	}

	public virtual void OnRespawn()
	{
	}
}
