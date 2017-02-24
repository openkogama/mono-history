public interface IInputSignalReceiver
{
	bool CurrentlyIsHot { get; }

	bool DefaultInput { get; }

	void UpdateSignal(bool isHot);

	void Reset();
}
