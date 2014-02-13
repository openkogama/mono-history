using System;
using MV.WorldObject;

public class CubeModelChangedEventArgs : EventArgs
{
	public readonly CubeAction Action;

	public readonly IntVector Pos;

	public CubeModelChangedEventArgs(CubeAction action, IntVector pos)
	{
		Action = action;
		Pos = pos;
	}

	public override string ToString()
	{
		return $"{Action} {Pos}";
	}
}
