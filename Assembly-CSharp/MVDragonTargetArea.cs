using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVDragonTargetArea : MVBlueprintBase
{
	private const string prefabPath = "Prefabs/Blueprints/HurtingFlames";

	private TriggerBoxEvents triggerBoxEvents;

	public MVDragonTargetArea(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Blueprints/HurtingFlames", worldObjects)
	{
		interactionFlags |= InteractionFlags.CanClone;
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
