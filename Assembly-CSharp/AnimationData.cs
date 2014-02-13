public class AnimationData
{
	private string state;

	private int timeStamp;

	public string State => state;

	public int TimeStamp => timeStamp;

	public AnimationData(string state, int timeStamp)
	{
		this.state = state;
		this.timeStamp = timeStamp;
	}
}
