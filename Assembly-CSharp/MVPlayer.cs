using System;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.GamePassSystem;
using MV.WorldObject.MetaData;
using MV.WorldObject.SpawnRoles;
using MV.WorldObject.Subscription;
using UnityEngine;
using UnityEngine.Events;

public class MVPlayer
{
	private int checkpointWOID = -1;

	protected PlayerPlanetDataRemote playerPlanetDataRemote;

	protected int level = 1;

	private MVTeam team = MVTeam.None;

	public Action OnGoldAmountChange;

	public UnityAction<int> OnLevelChanged;

	public UnityAction OnCheckpointReached;

	protected SpawnRolesManager spawnRolesManager;

	public int ProfileID { get; private set; }

	public bool IsTourist => ProfileID == 0;

	public int WoId => spawnRolesManager.SpawnRoleId;

	public int ActorNr { get; private set; }

	public string RegionCode { get; private set; }

	public BuildTarget BuildTarget { get; private set; }

	public PlayerPlanetDataRemote PlayerPlanetDataRemote
	{
		get
		{
			return playerPlanetDataRemote;
		}
		set
		{
			playerPlanetDataRemote = value;
		}
	}

	public UserProfileData UserProfileData { get; private set; }

	public SubscriptionRulesWrapper SubscriptionRules { get; private set; }

	public bool IsReady { get; private set; }

	public int Level
	{
		get
		{
			return level;
		}
		set
		{
			bool flag = level != value;
			level = value;
			if (flag && OnLevelChanged != null)
			{
				OnLevelChanged(level);
			}
		}
	}

	public MVTeam Team
	{
		get
		{
			return team;
		}
		set
		{
			team = value;
		}
	}

	public SpawnRolesManager SpawnRolesManager => spawnRolesManager;

	public MVPlayer(int actorNumber, int profileID, string regionCode, BuildTarget buildTarget, UserProfileData userProfileData, bool isReady)
	{
		ActorNr = actorNumber;
		ProfileID = profileID;
		BuildTarget = buildTarget;
		UserProfileData = userProfileData;
		SubscriptionRules = new SubscriptionRulesWrapper(userProfileData.SubscriptionData.SubscriptionType);
		if (profileID <= 0)
		{
			string newValue = TM._("Tourist");
			UserProfileData.UserName = UserProfileData.UserName.Replace("Tourist", newValue);
		}
		RegionCode = regionCode;
		IsReady = isReady;
	}

	public MVPlayer(int actorNumber, int profileID, int level, string regionCode, BuildTarget buildTarget, UserProfileData userProfileData, bool isReady, PlayerPlanetDataRemote playerPlanetDataRemote)
		: this(actorNumber, profileID, regionCode, buildTarget, userProfileData, isReady)
	{
		Level = level;
		PlayerPlanetDataRemote = playerPlanetDataRemote;
	}

	public void NotifyAvatarCreated(int id)
	{
		spawnRolesManager.OnAvatarCreated(id);
	}

	public bool IsOnSameTeam(MVPlayer other)
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() <= 1)
		{
			return false;
		}
		return team == other.Team;
	}

	public bool IsOnSameTeam(MVWorldObjectClient wo)
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() <= 1)
		{
			return false;
		}
		ITeamInteractorNPC teamInteractorNPC = wo as ITeamInteractorNPC;
		if (wo.OwnerActorNr == 0 && teamInteractorNPC != null)
		{
			return teamInteractorNPC.IsOnSameTeam(Team);
		}
		MVPlayer player = null;
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(wo.OwnerActorNr, out player))
		{
			return Team == player.Team;
		}
		return ActorNr == wo.OwnerActorNr;
	}

	public void SetCheckpoint(int woid)
	{
		if (MVGameControllerBase.WOCM.IsType(woid, WorldObjectType.CheckPoint))
		{
			checkpointWOID = woid;
			if (OnCheckpointReached != null)
			{
				OnCheckpointReached();
			}
			NotificationController.PushNotification(TM._("Reached new checkpoint!"));
		}
	}

	public MVCheckpoint GetCheckpoint()
	{
		if (checkpointWOID == -1)
		{
			return null;
		}
		if (MVGameControllerBase.WOCM.IsType(checkpointWOID, WorldObjectType.CheckPoint))
		{
			return MVGameControllerBase.WOCM.GetWorldObjectClient(checkpointWOID) as MVCheckpoint;
		}
		checkpointWOID = -1;
		return null;
	}

	public void ResetCheckpoint()
	{
		checkpointWOID = -1;
	}

	public int GetGameStat(GameStatCounterType gameStatCounterType)
	{
		return MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(gameStatCounterType, team, ActorNr);
	}

	public void SetReady()
	{
		if (IsReady)
		{
			Debug.LogError("Player already ready");
		}
		IsReady = true;
	}

	public void SetupSpawnRoleManager(ISpawnRoleChangeHandler spawnRoleChangeHandler, SpawnRolesRuntimeData spawnRolesRuntimeData)
	{
		spawnRolesManager = new SpawnRolesManager(spawnRoleChangeHandler, spawnRolesRuntimeData);
	}
}
