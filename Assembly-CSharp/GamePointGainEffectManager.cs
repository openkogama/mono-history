using System;

public static class GamePointGainEffectManager
{
	public static Action<int> OnGamePointGainEffectShown;

	public static Action<int> OnInGamePointGainEffectShown;

	public static Action<int> OnTierProgressBarGamePointGainEffectShown;

	private static int progressBarGamePointAmountShown;

	public static int GamePointAmountShown => progressBarGamePointAmountShown;

	public static void HaveShownGamePointGainEffect(int gamePointAmountShown)
	{
		progressBarGamePointAmountShown = gamePointAmountShown;
		if (OnGamePointGainEffectShown != null)
		{
			OnGamePointGainEffectShown(gamePointAmountShown);
		}
	}

	public static void HaveShownInGamePointGainEffect(int gamePointAmountShown)
	{
		if (OnInGamePointGainEffectShown != null)
		{
			OnInGamePointGainEffectShown(gamePointAmountShown);
		}
	}

	public static void HaveShownTierProgressBarGamePointGainEffect(int gamePointAmountShown)
	{
		progressBarGamePointAmountShown = gamePointAmountShown;
		if (OnTierProgressBarGamePointGainEffectShown != null)
		{
			OnTierProgressBarGamePointGainEffectShown(gamePointAmountShown);
		}
	}

	public static void PostResetCleanup()
	{
		OnGamePointGainEffectShown = null;
		OnInGamePointGainEffectShown = null;
		OnTierProgressBarGamePointGainEffectShown = null;
	}
}
