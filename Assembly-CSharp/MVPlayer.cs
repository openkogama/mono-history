using System.Collections;
using MV.WorldObject;

public class MVPlayer
{
	public delegate void OnScoreUpdatedDelegate(int score);

	public OnScoreUpdatedDelegate OnScoreUpdated;

	private MVTeam team = MVTeam.None;

	private int checkpointWOID = -1;

	private MVAvatar _avatar;

	private int _score;

	public int ProfileID { get; set; }

	public bool IsAnonymous => ProfileID == 0;

	public int ActorNr { get; set; }

	public string Username { get; set; }

	public string Password { get; set; }

	public int SilverAmount { get; set; }

	public int GoldAmount { get; set; }

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

	public int Score
	{
		get
		{
			return _score;
		}
		set
		{
			_score = value;
			if (OnScoreUpdated != null)
			{
				OnScoreUpdated(_score);
			}
		}
	}

	public int CollectibleCount
	{
		get
		{
			return (int)Avatar.CollectibleCount.Value;
		}
		private set
		{
			Avatar.CollectibleCount.Value = value;
		}
	}

	public Hashtable InitAvatarStatus { get; set; }

	public void SetCheckpoint(int woid)
	{
		if (MVGameController.Instance.WOCM.IsType(woid, WorldObjectType.CheckPoint))
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
		if (MVGameController.Instance.WOCM.IsType(checkpointWOID, WorldObjectType.CheckPoint))
		{
			return MVGameController.Instance.WOCM.GetWorldObjectClient(checkpointWOID) as MVCheckpoint;
		}
		checkpointWOID = -1;
		return null;
	}

	public void ResetCheckpoint()
	{
		checkpointWOID = -1;
	}

	public void ChangeScore(int amount)
	{
		int score = _score;
		score += amount;
		Score = ((score >= 0) ? score : 0);
	}

	public void Reset()
	{
		Score = 0;
		CollectibleCount = 0;
	}

	public void ChangeCollectibleCount(int amount)
	{
		int collectibleCount = CollectibleCount;
		collectibleCount += amount;
		CollectibleCount = collectibleCount;
	}
}
