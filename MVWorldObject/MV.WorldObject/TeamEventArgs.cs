using System;

namespace MV.WorldObject;

public class TeamEventArgs : EventArgs
{
	public MVTeam team;

	public TeamEventArgs(MVTeam team)
	{
		this.team = team;
	}
}
