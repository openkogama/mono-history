using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class MVPlayer
{
	private WorldObjectClientRef<MVAvatar> _avatar;

	private int checkpointWOID = -1;

	protected int level = 1;

	private MVTeam team = MVTeam.None;

	public UnityAction<int> OnLevelChanged;

	public UnityAction OnCheckpointReached;

	public int ProfileID { get; private set; }

	public bool IsTourist => ProfileID == 0;

	public int ActorNr { get; private set; }

	public string Username { get; private set; }

	public string RegionCode { get; private set; }

	public BuildTarget BuildTarget { get; private set; }

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
			if (_avatar != null)
			{
				Avatar.SetTeam();
			}
		}
	}

	public MVAvatar Avatar => _avatar.WorldObjectClient;

	public MVPlayer(int actorNumber, int profileID, string userName, string regionCode, BuildTarget buildTarget, bool isReady)
	{
		ActorNr = actorNumber;
		ProfileID = profileID;
		BuildTarget = buildTarget;
		if (profileID <= 0)
		{
			string newValue = TM._("Tourist");
			userName = userName.Replace("Tourist", newValue);
		}
		Username = userName;
		RegionCode = regionCode;
		IsReady = isReady;
	}

	public MVPlayer(int actorNumber, int profileID, string userName, int level, string regionCode, BuildTarget buildTarget, bool isReady)
		: this(actorNumber, profileID, userName, regionCode, buildTarget, isReady)
	{
		Level = level;
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

	public void SetAvatar(int worldObjectId)
	{
		_avatar = MVGameControllerBase.WOCM.GetWorldObjectClientRef<MVAvatar>(worldObjectId);
	}
}
