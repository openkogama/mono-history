public class LogicEvaluateInputSignalsOr : LogicEvaluateSignalComponentBase
{
	private bool orIsTrue;

	public override void UpdateSignal(bool isHot)
	{
		orIsTrue = orIsTrue || isHot;
	}

	public override bool GetResult()
	{
		bool result = orIsTrue;
		orIsTrue = false;
		return result;
	}
}
