public class PlayerData
{
	public MVPlayer player;

	public Friend friend;

	public int Score => player.GetGameStat(GameStatCounterType.Kill);

	public PlayerData(MVPlayer player, Friend friend)
	{
		this.player = player;
		this.friend = friend;
	}
}
