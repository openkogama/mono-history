using System.Collections.Generic;
using UnityEngine;

public class MVTeleporter : MVLogicObject
{
	private TriggerBoxEvents triggerBoxEvents;

	public List<TeleportEntry> teleportList = new List<TeleportEntry>();

	private GameObject endPointObject;

	private LineRenderer lineRenderer;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected Obj, but got Unknown
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/TelePorterObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
		endPointObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/TeleporterEndPointObject"), Position, Rotation);
		((Object)endPointObject).name = "TeleporterEndPoint";
		endPointObject.layer = LayerMask.NameToLayer("Logic");
		lineRenderer = endPointObject.AddComponent<LineRenderer>();
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
	}

	protected override void OnUpdate()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (teleportList.Count > 0)
		{
			List<TeleportEntry> list = new List<TeleportEntry>();
			foreach (TeleportEntry teleport in teleportList)
			{
				if (!teleport.Update())
				{
					list.Add(teleport);
				}
			}
			foreach (TeleportEntry item in list)
			{
				teleportList.Remove(item);
			}
		}
		lineRenderer.SetPosition(0, endPointObject.transform.position);
		lineRenderer.SetPosition(1, gameObject.transform.position);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.AvatarState == AvatarState.Playing && InputState)
		{
			int index = Random.Range(0, OutputLinkRefs.Count);
			if (MVGameController.Instance.WOCM.WorldObjects.ContainsKey(OutputLinkRefs[index].inputWOID))
			{
				TeleportEntry item = new TeleportEntry(e.instigatorWOID, this, MVGameController.Instance.WOCM.WorldObjects[OutputLinkRefs[index].inputWOID]);
				teleportList.Add(item);
			}
			else
			{
				Debug.LogWarning((object)"This shouldn't happen....bug in tele-porter object!");
			}
		}
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
	}

	public override bool OnClickHandler(EditorStateMachine esm, Collider collider)
	{
		if ((Object)(object)collider == (Object)(object)endPointObject.GetComponentInChildren<Collider>())
		{
			esm.PushState(EditorEvent.ESPlaceEndpoint);
			return true;
		}
		return base.OnClickHandler(esm, collider);
	}
}
