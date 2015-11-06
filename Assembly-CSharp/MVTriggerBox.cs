using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVTriggerBox : MVLogicObject
{
	private const string prefabPath = "Prefabs/TriggerBoxObject";

	private TriggerBoxEvents triggerBoxEvents;

	private GameObject audioGO;

	private AudioLogicCube audioLC;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset => new Vector3(2f, 0f, 0f);

	public MVTriggerBox(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/TriggerBoxObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		audioGO = (GameObject)Object.Instantiate(Resources.Load("Audio/AudioPrefabs/TriggerBoxSound"), Vector3.zero, Quaternion.identity);
		audioGO.transform.parent = gameObject.transform;
		audioLC = audioGO.GetComponentInChildren<AudioLogicCube>();
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, Vector3.one * 2f);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		MVGameControllerBase.Game.TriggerBoxEnter(Id, e.instigatorWOID);
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		MVGameControllerBase.Game.TriggerBoxExit(Id, e.instigatorWOID);
	}

	public void OnEnter(MVPlayer player)
	{
		if (player != MVGameControllerBase.Game.LocalPlayer)
		{
		}
	}

	public void OnExit(MVPlayer player)
	{
		if (player != MVGameControllerBase.Game.LocalPlayer)
		{
		}
	}

	public void OnStayBegin(int actorNr)
	{
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = true;
		}
		audioLC.Play(on: true);
	}

	public void OnStayEnd()
	{
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = false;
		}
		audioLC.Play(on: false);
	}

	public override void Destroy()
	{
		triggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit -= triggerBoxEvents_TriggerExit;
		base.Destroy();
	}
}
