public class ScoreActorEntry : IScoreEntry
{
	public readonly int actorNumber;

	public readonly int counter;

	public ScoreActorEntry(int actorNumber, int counter)
	{
		this.actorNumber = actorNumber;
		this.counter = counter;
	}

	public override string ToString()
	{
		return $"ActorNumber: {actorNumber}. Counter: {counter}.";
	}
}
