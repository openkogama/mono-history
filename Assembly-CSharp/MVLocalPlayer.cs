using System;
using System.Collections.Generic;
using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.Mediator;
using MV.Common;
using MV.WorldObject.GamePassSystem;
using MV.WorldObject.MetaData;
using MV.WorldObject.SpawnRoles;
using UnityEngine;
using UnityEngine.Events;

public abstract class MVLocalPlayer : MVPlayer
{
	public enum PlanetOwnershipType
	{
		None,
		Editor,
		Owner
	}

	protected SpawnRolesMetaData spawnRolesMetaData;

	private PlayerPlanetData playerPlanetData;

	private BoostController boostController = new BoostController();

	private int defaultBodyWoId = -1;

	public Action OnInitializeLeveling;

	private readonly SpawnRoleDataMediator spawnRoleDataMediator = new SpawnRoleDataMediator();

	protected XPProgress xpProgress;

	private int planetOwnershipTypeID;

	public XPProgress.OnXPProgressDataDelegate OnXPProgressData;

	protected int joinTime;

	protected const float respawnDuration = 4f;

	protected float respawnTime;

	protected const float reviveTimeout = 10f;

	private int oldLevel;

	public MVBody Body => MVGameControllerBase.WOCM.GetWorldObjectClient<MVBody>(defaultBodyWoId);

	public int DefaultSpawnRoleId => spawnRolesMetaData.spawnRolesDefaultTypeWoIDMap[DefaultSpawnRoleType.DefaultPlayModeSpawnRole];

	public SpawnRoleDataMediator SpawnRoleDataMediator => spawnRoleDataMediator;

	public PlayerPlanetData PlayerPlanetData
	{
		get
		{
			return playerPlanetData;
		}
		set
		{
			playerPlanetData = value;
			playerPlanetDataRemote = new PlayerPlanetDataRemote(playerPlanetData.highScoreGamePoints, playerPlanetData.gamePassTier);
		}
	}

	public BoostController BoostController
	{
		get
		{
			return boostController;
		}
		private set
		{
			boostController = value;
		}
	}

	public int DefaultBodyWoId => defaultBodyWoId;

	public int PlanetOwnershipTypeID
	{
		get
		{
			if (MVGameControllerBase.GameMode != MVGameMode.Edit)
			{
				throw new Exception("There are currently no way to access MVLocalPlayer:PlanetOwnership in play mode. Server side refactoring is required to fix this.");
			}
			return planetOwnershipTypeID;
		}
		private set
		{
			planetOwnershipTypeID = value;
		}
	}

	public PlanetOwnershipType PlanetOwnership => (PlanetOwnershipType)PlanetOwnershipTypeID;

	public XPProgressData XPProgressData => xpProgress.XPProgressData;

	public int JoinTime => joinTime;

	public bool CanGetXPProgressData => xpProgress != null;

	public float RespawnDuration => 4f;

	public float RespawnTime
	{
		get
		{
			return respawnTime;
		}
		set
		{
			respawnTime = value;
		}
	}

	public float ReviveTimeout => 10f;

	public MVLocalPlayer(int actorNumber, int profileID, string regionCode, int planetOwnershipTypeID, UserProfileData userProfileData)
		: base(actorNumber, profileID, regionCode, MVGameControllerBase.BuildTarget, userProfileData, isReady: false)
	{
		OnLevelChanged = (UnityAction<int>)Delegate.Combine(OnLevelChanged, new UnityAction<int>(OnLevelChangedLocal));
		PlanetOwnershipTypeID = planetOwnershipTypeID;
		joinTime = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
	}

	private void SpawnRoleModeOnOnChange(SpawnRoleModeType value)
	{
		if (value == SpawnRoleModeType.Playing)
		{
			MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.OnChange -= SpawnRoleModeOnOnChange;
			MVGameControllerBase.OperationRequests.IncrementStatRequest(IncrementStatRequestType.PlayerHasEnteredWorldFirstTime);
		}
	}

	public void SetupPlayerWorldObjects(int defaultBodyWoId, SpawnRolesRuntimeData spawnRolesRuntimeData)
	{
		this.defaultBodyWoId = defaultBodyWoId;
		SpawnRoleChangeHandlerLocal spawnRoleChangeHandler = new SpawnRoleChangeHandlerLocal(spawnRoleDataMediator);
		SetupSpawnRoleManager(spawnRoleChangeHandler, spawnRolesRuntimeData);
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.OnChange += SpawnRoleModeOnOnChange;
	}

	public virtual void InitializeLeveling(InitialLevelData initialLevelData)
	{
		level = initialLevelData.Level;
		xpProgress = new XPProgress(this, initialLevelData);
		if (OnInitializeLeveling != null)
		{
			OnInitializeLeveling();
		}
	}

	public void SetSpawnRoleMetaData(SpawnRolesMetaData spawnRolesMetaData)
	{
		this.spawnRolesMetaData = spawnRolesMetaData;
	}

	public void SetActiveSpawnRole(int existingAvatarWoId)
	{
		if (existingAvatarWoId == SpawnRolesManager.SpawnRoleId)
		{
			throw new Exception("Trying to set active spawn role to already active spawnRole");
		}
		SuspendCurrentSpawnRole();
		MVGameControllerBase.OperationRequests.SetActiveSpawnRole(existingAvatarWoId);
	}

	public void CreateSpawnRole(int avatarSpawnerWoId)
	{
		Debug.LogWarning("If this fails then remember to un-suspend. This can happen in build mode if the avatar spawner is removed by another player.");
		SuspendCurrentSpawnRole();
		MVGameControllerBase.OperationRequests.CreateSpawnRole(avatarSpawnerWoId);
	}

	public void CreateSpawnRoleFailed()
	{
		UnSuspendCurrentSpawnRole();
	}

	private void SuspendCurrentSpawnRole()
	{
		ISpawnRoleLocal spawnRoleLocal = (ISpawnRoleLocal)MVGameControllerBase.WOCM.GetWorldObject(SpawnRolesManager.SpawnRoleId);
		spawnRoleLocal.Suspend();
	}

	private void UnSuspendCurrentSpawnRole()
	{
		ISpawnRoleLocal spawnRoleLocal = (ISpawnRoleLocal)MVGameControllerBase.WOCM.GetWorldObject(SpawnRolesManager.SpawnRoleId);
		spawnRoleLocal.UnSuspend();
	}

	public void AddXp(int currentPlayerXP, XPRewardType typeId, int xpDelta, int memberCount)
	{
		if (LevelingManager.IsInitialized)
		{
			xpProgress.Update(currentPlayerXP, typeId, xpDelta, memberCount);
		}
	}

	protected void SendXpProgressEvent(XPProgressData xpProgressData)
	{
		if (OnXPProgressData != null)
		{
			OnXPProgressData(xpProgressData);
		}
		NotificationController.PushNotification(string.Format(TM._("You gained {0} XP!"), xpProgressData.XPDelta));
		MVGameControllerBase.GameEventManager.NotifyXPDeltaAmount(xpProgressData.XPDelta);
	}

	protected void OnLevelChangedLocal(int level)
	{
		MVGameControllerBase.OperationRequests.LocalPlayerLevelChanged(level);
		if (oldLevel != 0)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)4, level);
			Dictionary<object, object> data = dictionary;
			NotificationController.OnNotificationReceived(NotificationType.LevelUp, data);
		}
		oldLevel = level;
	}

	public virtual void Destroy()
	{
		xpProgress.Destroy();
	}
}
