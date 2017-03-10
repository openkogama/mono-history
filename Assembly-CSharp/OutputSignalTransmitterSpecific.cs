using MV.WorldObject;

public class OutputSignalTransmitterSpecific
{
	protected readonly int woId = -1;

	public OutputSignalTransmitterSpecific(int woId)
	{
		this.woId = woId;
	}

	public void Send(int hotIndex)
	{
		MVWorldObject worldObject = MVGameControllerBase.WOCM.GetWorldObject(woId);
		for (int i = 0; i < worldObject.OutputLinkRefs.Count; i++)
		{
			bool flag = i == hotIndex;
			ILogicWorldObject logicWorldObject = (ILogicWorldObject)MVGameControllerBase.WOCM.GetWorldObject(worldObject.OutputLinkRefs[i].inputWOID);
			logicWorldObject.InputSignalReceiver.UpdateSignal(flag);
			worldObject.OutputLinkRefs[i].isSet = flag;
		}
	}
}
