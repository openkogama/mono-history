namespace MV.WorldObject;

public static class ScoreCalculation
{
	private const int KILL_POINTS = 1;

	private const int SUICIDE_POINTS = -1;

	private const int FLAG_CAPTURE_POINTS = 5;

	public static void KillScore(int victimActorID, MVTeam victimTeam, int killerActorID, MVTeam killerTeam, int teamCount, ref int killerScore, ref int teamScore)
	{
		if (!TeamKill(victimActorID, victimTeam, killerActorID, killerTeam, teamCount))
		{
			killerScore += ((killerActorID != victimActorID) ? 1 : (-1));
			if (victimActorID != killerActorID)
			{
				teamScore++;
			}
		}
		if (killerScore < 0)
		{
			killerScore = 0;
		}
		if (teamScore < 0)
		{
			teamScore = 0;
		}
	}

	private static bool TeamKill(int victimActorID, MVTeam victimTeam, int killerActorID, MVTeam killerTeam, int teamCount)
	{
		if (teamCount > 1 && victimActorID != killerActorID)
		{
			return victimTeam == killerTeam;
		}
		return false;
	}
}
