using System;
using MV.WorldObject;

public class OnCounterTypeChangedArgs : EventArgs
{
	public readonly int count = 0;

	public readonly GameStatCounterType counterType;

	public readonly int actorNumber;

	public readonly MVTeam team;

	public readonly int otherID = -1;

	public OnCounterTypeChangedArgs(int count, GameStatCounterType counterType, int actorNumber, MVTeam team, int otherID)
	{
		this.count = count;
		this.counterType = counterType;
		this.actorNumber = actorNumber;
		this.team = team;
		this.otherID = otherID;
	}

	public override string ToString()
	{
		return $"CounterType: {counterType}. Value: {count}. ActorNumber: {actorNumber}. Team: {team}. OtherID: {otherID}.";
	}
}
