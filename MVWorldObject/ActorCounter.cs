using MV.WorldObject;

public class ActorCounter
{
	private int count;

	public int Count => count;

	public ActorCounter(int value)
	{
		count = value;
	}

	public ActorCounter(BytePacker bp)
	{
		count = bp.ReadInt32();
	}

	public int Increment(int value)
	{
		count += value;
		return count;
	}

	public override string ToString()
	{
		return count.ToString();
	}

	public byte[] ToByteArray()
	{
		BytePacker bytePacker = new BytePacker();
		bytePacker.Write(count);
		return bytePacker.ToArray();
	}
}
