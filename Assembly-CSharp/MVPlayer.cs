using MV.WorldObject;

public class MVPlayer
{
	public delegate void OnLevelChangedDelegate(int level);

	private int checkpointWOID = -1;

	protected int level = 1;

	private MVTeam team;

	public OnLevelChangedDelegate OnLevelChanged;

	private MVAvatar _avatar;

	public int ProfileID { get; private set; }

	public bool IsAnonymous => ProfileID == 0;

	public int ActorNr { get; private set; }

	public string Username { get; private set; }

	public string RegionCode { get; private set; }

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

	public int PlanetOwnershipTypeID { get; set; }

	public MVAvatar Avatar
	{
		get
		{
			return _avatar;
		}
		set
		{
			_avatar = value;
		}
	}

	public MVPlayer(int actorNumber, int profileID, string userName, string regionCode)
	{
		ActorNr = actorNumber;
		ProfileID = profileID;
		if (profileID <= 0)
		{
			string newValue = TM._("Tourist");
			userName = userName.Replace("Tourist", newValue);
		}
		Username = userName;
		RegionCode = regionCode;
	}

	public MVPlayer(int actorNumber, int profileID, string userName, int level, string regionCode)
		: this(actorNumber, profileID, userName, regionCode)
	{
		Level = level;
	}

	public void SetCheckpoint(int woid)
	{
		if (MVGameControllerBase.WOCM.IsType(woid, WorldObjectType.CheckPoint))
		{
			checkpointWOID = woid;
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
}
