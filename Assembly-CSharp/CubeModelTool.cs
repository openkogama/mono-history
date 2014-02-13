internal class CubeModelTool : IState
{
	protected bool waitForMouseUp;

	public CubeModelingEvent StateType { get; protected set; }

	public virtual bool CursorVisible
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	public void SetStateType(CubeModelingEvent stateTypeEvent)
	{
		StateType = stateTypeEvent;
	}

	public virtual void Enter(CubeModelingStateMachine esm)
	{
	}

	public virtual void Execute(CubeModelingStateMachine e)
	{
	}

	public virtual void Exit(CubeModelingStateMachine esm)
	{
	}

	public void Enter(FSMEntity e)
	{
		Enter((CubeModelingStateMachine)e);
	}

	public void Execute(FSMEntity e)
	{
		Execute((CubeModelingStateMachine)e);
	}

	public void Exit(FSMEntity e)
	{
		Exit((CubeModelingStateMachine)e);
	}

	public virtual void HideCursor()
	{
	}
}
