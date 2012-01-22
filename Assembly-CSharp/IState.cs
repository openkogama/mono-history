public interface IState
{
	void Enter(FSMEntity e);

	void Execute(FSMEntity e);

	void Exit(FSMEntity e);
}
