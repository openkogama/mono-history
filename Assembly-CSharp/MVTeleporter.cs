using System.Collections;
using System.Collections.Generic;
using Localize;
using UnityEngine;

public class MVTeleporter : MVLogicObject
{
	private const string prefabPath = "Prefabs/TelePorterObject";

	private TriggerBoxEvents triggerBoxEvents;

	private List<MVAvatar> avatarIgnoreList = new List<MVAvatar>();

	private TeleportAvatar teleportAvatarPrefab;

	private ParticleSystem teleportParticles;

	private MVTeleporter target;

	public override Vector3 InputConnectorOffset
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(-1.5f, 0f, 0f);
		}
	}

	public override Vector3 ObjectConnectorOffset
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(0f, 1.5f, 0f);
		}
	}

	public override Quaternion ObjectConnectorRotation
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return Quaternion.LookRotation(Vector3.down);
		}
	}

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public override bool HasObjectConnector => false;

	public MVTeleporter Target
	{
		set
		{
			target = value;
		}
	}

	public MVTeleporter(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/TelePorterObject", worldObjects)
	{
		teleportAvatarPrefab = Resources.Load("Prefabs/Logic/TeleportAvatar", typeof(TeleportAvatar)) as TeleportAvatar;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		teleportParticles = gameObject.GetComponentInChildren<ParticleSystem>();
		interactionFlags &= ~InteractionFlags.CanClone;
	}

	public override void Initialize()
	{
		base.Initialize();
		OnInputLinkChanged();
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref TextSlotIndex errorTextIndex)
	{
		return worldObjectClientManager.GetWorldObjectClient(groupId)?.Delete(worldObjectClientManager, ref errorTextIndex) ?? false;
	}

	public override bool ValidateObjectLinkTarget(MVWorldObjectClient wo)
	{
		return wo is MVTeleporter;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(Mathf.Round(position.x / gridSize), Mathf.Round(position.y / gridSize), Mathf.Round(position.z / gridSize));
		return val * gridSize;
	}

	public override void OnInputLinkChanged()
	{
		base.OnInputLinkChanged();
		OnInputStateChanged();
	}

	public override void OnInputStateChanged()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.OnInputStateChanged();
		bool flag = InputLinkRefs.Count == 0 || InputState;
		teleportParticles.startColor = ((!flag) ? new Color(1f, 0.5f, 0f) : new Color(0f, 0.5f, 1f));
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (InputLinkRefs.Count != 0 && !InputState)
		{
			return;
		}
		MVTeleporter mVTeleporter = target;
		if (MVGameController.Instance.WOCM.GetWorldObjectClient(e.instigatorWOID) is MVAvatarLocal)
		{
			MVAvatarLocal mVAvatarLocal = MVGameController.Instance.WOCM.GetWorldObjectClient(e.instigatorWOID) as MVAvatarLocal;
			if (!mVAvatarLocal.IsEnteringVehicle && !avatarIgnoreList.Contains(mVAvatarLocal))
			{
				TeleportAvatar teleportAvatar = Object.Instantiate((Object)(object)teleportAvatarPrefab, transform.position, Quaternion.identity) as TeleportAvatar;
				teleportAvatar.avatar = mVAvatarLocal;
				teleportAvatar.targetPosition = target.WorldPosition;
				teleportAvatar.originPosition = transform.position;
				mVTeleporter.avatarIgnoreList.Add(mVAvatarLocal);
			}
		}
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		if (MVGameController.Instance.WOCM.GetWorldObjectClient(e.instigatorWOID) is MVAvatarLocal)
		{
			MVAvatarLocal avatarLocal = MVGameController.Instance.WOCM.AvatarLocal;
			if (e.instigatorWOID == avatarLocal.Id)
			{
				avatarIgnoreList.Remove(avatarLocal);
			}
		}
	}
}
