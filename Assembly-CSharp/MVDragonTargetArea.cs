using System.Collections.Generic;
using UnityEngine;

public class MVDragonTargetArea : MVBlueprintBase
{
	private const string prefabPath = "Prefabs/Blueprints/HurtingFlames";

	private TriggerBoxEvents triggerBoxEvents;

	public MVDragonTargetArea(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Blueprints/HurtingFlames", worldObjects)
	{
		interactionFlags |= InteractionFlags.CanClone;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if (triggerBoxEvents != null)
		{
			triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
			triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		}
		else
		{
			Debug.LogError("A TriggerBoxEvents object is missing in PickupItem type: " + GetType().Name);
		}
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		gameObject.GetComponent<DragonTargetArea>().isAvatarInside = true;
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		gameObject.GetComponent<DragonTargetArea>().isAvatarInside = false;
	}
}
