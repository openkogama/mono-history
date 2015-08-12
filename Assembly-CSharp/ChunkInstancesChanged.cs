using System;
using MV.WorldObject;

public class ChunkInstancesChanged : EventArgs
{
	public enum ChangeType
	{
		Added,
		Removed,
		Clear
	}

	public readonly ChangeType changeType;

	public readonly IntVector chunkPos;

	public ChunkInstancesChanged(ChangeType changeType, IntVector chunkPos)
	{
		this.changeType = changeType;
		this.chunkPos = chunkPos;
	}
}
