using System;
using MV.Common;

public static class FakeGamePointGainEffectManager
{
	public static Action<int> OnFakeGamePointGainEffect;

	public static void FakeGainEffect(int amountOfGamePoints)
	{
		if (OnFakeGamePointGainEffect != null && ShouldShowFakeGainEffect())
		{
			OnFakeGamePointGainEffect(amountOfGamePoints);
		}
	}

	private static bool ShouldShowFakeGainEffect()
	{
		bool flag = MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit;
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		return flag || isTouristSession;
	}
}
