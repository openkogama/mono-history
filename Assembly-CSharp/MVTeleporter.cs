using System.Collections.Generic;
using UnityEngine;

public class MVTeleporter : MVLogicObject
{
	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private List<MVAvatar> avatarIgnoreList = new List<MVAvatar>();

	private MVTeleporterObject teleportObject;

	private TeleportAvatar teleportAvatarPrefab;

	private MVTeleporter target;

	private UseInteractor useInteractor;

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
		teleportObject = (MVTeleporterObject)component;
		teleportAvatarPrefab = PrefabPool.Instance.TeleportAvatarPrefab;
		teleportObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		teleportObject.TriggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		interactionFlags &= ~InteractionFlags.CanClone;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseStars;
		interactionFlags |= InteractionFlags.CanUseTeam;
	}

	public override void Initialize()
	{
		SetupUseInteractor();
		base.Initialize();
		useInteractor.UpdateData(Data);
		SetupCulling(teleportObject.visualRoot).Radius = 4f;
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		ParticleSystem.MainModule main = teleportObject.ParticleSystem.main;
		main.startSizeMultiplier = 0.8f;
	}

	private void SetupUseInteractor()
	{
		useInteractor = new UseInteractor(this, teleportObject.useInteractionRotator, reset: false, teleportObject.TriggerBoxEvents.Collider, DoTeleport);
		GameCoinLogic useRequirement = new GameCoinLogic(teleportObject.useInteractionRotator, hasUseButtonWhenFree: false);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(teleportObject.useInteractionRotator, hasUseButtonWhenFree: false);
		StarRequirement useRequirement3 = new StarRequirement(teleportObject.useInteractionRotator, hasUseButtonWhenFree: false);
		TeamRequirement useRequirement4 = new TeamRequirement(teleportObject.TintObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
		useInteractor.AddRequirement(useRequirement);
		useInteractor.AddRequirement(useRequirement3);
		useInteractor.AddRequirement(useRequirement4);
		teleportObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		teleportObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		useInteractor.UpdateData(Data);
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

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		bool flag = InputLinkRefs.Count == 0 || InputState;
		if ((useInteractor.EvaluateRequirementsUsability() & UseGUIResult.CanAfford) != 0)
		{
			MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
			if (e.instigatorWOID == avatarLocal.Id)
			{
				avatarIgnoreList.Remove(avatarLocal);
			}
		}
		if (flag && (useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
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
			avatarLocal.LeaveVehicle(leaveBecauseOfServer: false);
		}
		if (!avatarIgnoreList.Contains(avatarLocal))
		{
			TeleportAvatar teleportAvatar = Object.Instantiate(teleportAvatarPrefab, transform.position, Quaternion.identity);
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
			if (useInteractor != null)
			{
				teleportObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
				teleportObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
				useInteractor.OnDestroy(Data);
				useInteractor = null;
			}
			base.Destroy();
			isDestroyed = true;
		}
	}
}
