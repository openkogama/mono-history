using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;

public class InputSignalReceiverClient : InputSignalReceiverBase
{
	public override bool CurrentlyIsHot { get; set; }

	public InputSignalReceiverClient(MVWorldObject worldObject, LogicEvaluateSignalComponentBase logicEvaluateSignalComponentBase, bool defaultInput, LogicObjectManager logicObjectManager)
		: base(worldObject, logicEvaluateSignalComponentBase, defaultInput, logicObjectManager)
	{
		CurrentlyIsHot = (ObscuredBool)worldObject.RunTimeData.GetObscuredType("iH");
		firstFrame = false;
	}
}
