public class LogicEvaluateInputSignalsAnd : LogicEvaluateSignalComponentBase
{
	private bool andIsTrue = true;

	public override void UpdateSignal(bool isHot)
	{
		andIsTrue = andIsTrue && isHot;
	}

	public override bool GetResult()
	{
		bool result = andIsTrue;
		andIsTrue = true;
		return result;
	}
}
