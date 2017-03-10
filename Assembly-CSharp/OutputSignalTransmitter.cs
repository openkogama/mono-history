using MV.WorldObject;

public class OutputSignalTransmitter
{
	protected readonly int woId = -1;

	public OutputSignalTransmitter(int woId)
	{
		this.woId = woId;
	}

	public void Send(bool isHot)
	{
		MVWorldObject worldObject = MVGameControllerBase.WOCM.GetWorldObject(woId);
		foreach (Link outputLinkRef in worldObject.OutputLinkRefs)
		{
			ILogicWorldObject logicWorldObject = (ILogicWorldObject)MVGameControllerBase.WOCM.GetWorldObject(outputLinkRef.inputWOID);
			logicWorldObject.InputSignalReceiver.UpdateSignal(isHot);
			outputLinkRef.isSet = isHot;
		}
	}
}
