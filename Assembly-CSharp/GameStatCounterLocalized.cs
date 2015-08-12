public class GameStatCounterLocalized
{
	public readonly string txt;

	public readonly GameStatCounterType gameStatCounterType;

	public GameStatCounterLocalized(GameStatCounterType gameStatCounterType, string txt)
	{
		this.gameStatCounterType = gameStatCounterType;
		this.txt = txt;
	}
}
