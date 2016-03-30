using System.Collections.Generic;
using UnityEngine;

public class MVTeleporter : MVLogicObject
{
	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private List<MVAvatar> avatarIgnoreList = new List<MVAvatar>();

	private TeleportAvatar teleportAvatarPrefab;

	private MVTeleporter target;

	private UseInteractor useInteractor;

	private TriggerBoxEvents triggerBoxEvents;

	private ParticleSystem teleportParticles;

	private Vector3 gameCoinDisplayObjectOffset = new Vector3(0f, 1.5f, 0f);

	private bool isDestroyed;

	public override Vector3 InputConnectorOffset => new Vector3(-1.5f, 0f, 0f);

	public override Vector3 ObjectConnectorOffset => new Vector3(0f, 1.5f, 0f);

	public override Quaternion ObjectConnectorRotation => Quaternion.LookRotation(Vector3.down);

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

	public MVTeleporter(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVTeleporterPrefab, worldObjects)
	{
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		teleportParticles = gameObject.GetComponentInChildren<ParticleSystem>();
		teleportAvatarPrefab = PrefabPool.Instance.TeleportAvatarPrefab;
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		interactionFlags &= ~InteractionFlags.CanClone;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseStars;
		useInteractor = new UseInteractor(Id, gameObject, reset: false, triggerBoxEvents.Collider, DoTeleport);
		GameCoinLogic useRequirement = new GameCoinLogic(gameObject, gameCoinDisplayObjectOffset, hasUseButtonWhenFree: false);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(gameObject, hasUseButtonWhenFree: false);
		StarRequirement useRequirement3 = new StarRequirement(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
		useInteractor.AddRequirement(useRequirement);
		useInteractor.AddRequirement(useRequirement3);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		useInteractor.UpdateData(Data);
	}

	public override void Initialize()
	{
		base.Initialize();
		useInteractor.UpdateData(Data);
		OnInputLinkChanged();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		return worldObjectClientManager.GetWorldObjectClient(groupId)?.Delete(worldObjectClientManager, ref errorText) ?? false;
	}

	public override bool ValidateObjectLinkTarget(MVWorldObjectClient wo)
	{
		return wo is MVTeleporter;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 closestGridPoint = base.GetClosestGridPoint(gridSize, position);
		closestGridPoint.y = Mathf.Round(position.y / gridSize) * gridSize;
		return closestGridPoint;
	}

	public override void OnInputLinkChanged()
	{
		base.OnInputLinkChanged();
		OnInputStateChanged();
	}

	public override void OnInputStateChanged()
	{
		base.OnInputStateChanged();
		bool flag = InputLinkRefs.Count == 0 || InputState;
		teleportParticles.startColor = ((!flag) ? new Color(1f, 0.5f, 0f) : new Color(0f, 0.5f, 1f));
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if ((InputLinkRefs.Count == 0 || InputState) && (useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
		{
			DoTeleport(e.instigatorWOID);
		}
	}

	private bool DoTeleport(int instigatorWOID)
	{
		MVTeleporter mVTeleporter = target;
		if (!(MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorWOID) is MVAvatarLocal))
		{
			return false;
		}
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		if (avatarLocal.IsEnteringVehicle)
		{
			return false;
		}
		if (avatarLocal.IsSeated)
		{
			avatarLocal.LeaveVehicle();
		}
		if (!avatarIgnoreList.Contains(avatarLocal))
		{
			TeleportAvatar teleportAvatar = Object.Instantiate(teleportAvatarPrefab, transform.position, Quaternion.identity) as TeleportAvatar;
			teleportAvatar.avatar = avatarLocal;
			teleportAvatar.targetPosition = target.WorldPosition;
			teleportAvatar.originPosition = transform.position;
			mVTeleporter.avatarIgnoreList.Add(avatarLocal);
		}
		return true;
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		if (MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID) is MVAvatarLocal)
		{
			MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
			if (e.instigatorWOID == avatarLocal.Id)
			{
				avatarIgnoreList.Remove(avatarLocal);
			}
		}
	}

	public override void Destroy()
	{
		if (!isDestroyed)
		{
			triggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
			triggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
			useInteractor.OnDestroy(Data);
			base.Destroy();
			isDestroyed = true;
		}
	}
}
