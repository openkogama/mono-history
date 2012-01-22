using MV.WorldObject;
using UnityEngine;

public class MVTriggerBox : MVLogicObject, WorldObjectWithSettings, WorldObjectWithLogicReset
{
	private TriggerBoxEvents triggerBoxEvents;

	private GameObject audioGO;

	private AudioLogicCube audioLC;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(2f, 0f, 0f);
		}
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, Vector3.one * 2f);
	}

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected Obj, but got Unknown
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/TriggerBoxObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		audioGO = (GameObject)Object.Instantiate(Resources.Load("Audio/AudioPrefabs/TriggerBoxSound"), Vector3.zero, Quaternion.identity);
		audioGO.transform.parent = gameObject.transform;
		audioLC = audioGO.GetComponentInChildren<AudioLogicCube>();
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

	public void OnEnter(MVPlayer player)
	{
		if (player != MVGameController.Instance.WOCM.LocalPlayer)
		{
		}
	}

	public void OnExit(MVPlayer player)
	{
		if (player != MVGameController.Instance.WOCM.LocalPlayer)
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
	}
}
