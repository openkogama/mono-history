public interface IInputHandler
{
	int Priority { get; }

	bool HandleInput();
}
