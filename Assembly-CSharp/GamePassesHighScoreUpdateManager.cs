using System;
using MV.WorldObject.GamePassSystem;

public static class GamePassesHighScoreUpdateManager
{
	public static Action<HighScoreDatas> OnHighScoreUpdate;

	public static void UpdateHigscore(HighScoreDatas newHighScore)
	{
		if (OnHighScoreUpdate != null)
		{
			OnHighScoreUpdate(newHighScore);
		}
	}
}
