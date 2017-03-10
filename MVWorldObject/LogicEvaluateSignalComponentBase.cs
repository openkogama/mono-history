public abstract class LogicEvaluateSignalComponentBase
{
	public int evaluatedSignals;

	public int signalsToEvaluate;

	public abstract bool GetResult();

	public abstract void UpdateSignal(bool isHot);
}
