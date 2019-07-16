using System.Collections.Generic;
using UnityEngine;

public class MVTeleporter : MVLogicObject
{
	private const UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private List<int> avatarIgnoreList = new List<int>();

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
		interactionFlags |= InteractionFlags.CanUseGameRank;
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
		GameRankRequirement gameRankRequirement = new GameRankRequirement(teleportObject.useInteractionRotator, this, hasUseButtonWhenFree: false);
		gameRankRequirement.OnDataUpdate(Data, id);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(teleportObject.useInteractionRotator, hasUseButtonWhenFree: false);
		StarRequirement useRequirement3 = new StarRequirement(teleportObject.useInteractionRotator, hasUseButtonWhenFree: false);
		TeamRequirement useRequirement4 = new TeamRequirement(teleportObject.TintObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
		useInteractor.AddRequirement(useRequirement);
		useInteractor.AddRequirement(useRequirement3);
		useInteractor.AddRequirement(useRequirement4);
		useInteractor.AddRequirement(gameRankRequirement);
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
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID);
			if (worldObjectClient is MVAvatarLocal)
			{
				avatarIgnoreList.Remove(worldObjectClient.Id);
			}
		}
		if (flag && (useInteractor.EvaluateRequirementsUsability() & (UseGUIResult.CanAfford | UseGUIResult.CannotAfford)) == 0)
		{
			DoTeleport(e.instigatorWOID);
		}
	}

	private bool DoTeleport(int instigatorWOID)
	{
		Debug.Log("DoTeleport");
		MVTeleporter mVTeleporter = target;
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorWOID);
		if (!(worldObjectClient is MVAvatarLocal))
		{
			return false;
		}
		Debug.Log("DoTeleport2");
		MVAvatarLocal mVAvatarLocal = (MVAvatarLocal)worldObjectClient;
		if (mVAvatarLocal.IsEnteringVehicle)
		{
			return false;
		}
		if (mVAvatarLocal.IsSeated)
		{
			mVAvatarLocal.LeaveVehicle(leaveBecauseOfServer: false);
		}
		if (!mVAvatarLocal.IsSpawnRoleActive())
		{
			return false;
		}
		if (!avatarIgnoreList.Contains(mVAvatarLocal.Id))
		{
			TeleportAvatar teleportAvatar = Object.Instantiate(teleportAvatarPrefab, transform.position, Quaternion.identity);
			teleportAvatar.avatar = mVAvatarLocal;
			teleportAvatar.targetPosition = target.WorldPosition;
			teleportAvatar.originPosition = transform.position;
			mVTeleporter.avatarIgnoreList.Add(mVAvatarLocal.Id);
			Debug.LogWarning("actor ignore list should use ids");
		}
		return true;
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID);
		if (worldObjectClient is MVAvatarLocal)
		{
			avatarIgnoreList.Remove(worldObjectClient.Id);
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
