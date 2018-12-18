using MV.WorldObject;

public class LogicObjectManagerClient : LogicObjectManager
{
	public LogicObjectManagerClient(int timeStamp, bool trackLoops)
		: base(timeStamp, trackLoops)
	{
	}

	public int OnLinkAdded(Link link, IWorldObjectManager worldObjectManager)
	{
		return LogicObjectManager.ResetChunk(link.inputWOID, worldObjectManager);
	}

	public int OnLinkRemoved(Link link, IWorldObjectManager worldObjectManager)
	{
		int num = LogicObjectManager.ResetChunk(link.inputWOID, worldObjectManager);
		int num2 = LogicObjectManager.ResetChunk(link.outputWOID, worldObjectManager);
		return num + num2;
	}

	public void Clear()
	{
		logicWorldObjects.Clear();
	}
}
