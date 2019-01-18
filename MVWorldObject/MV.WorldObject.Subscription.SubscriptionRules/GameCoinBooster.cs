namespace MV.WorldObject.Subscription.SubscriptionRules;

public class GameCoinBooster : SubscriptionRule
{
	public int GameCoinBoost { get; private set; }

	public GameCoinBooster(int gameCoinBoost)
	{
		GameCoinBoost = gameCoinBoost;
	}

	public int GetBoostedGameCoins(int gameCoins)
	{
		return gameCoins + gameCoins * (int)((float)GameCoinBoost / 100f);
	}
}
