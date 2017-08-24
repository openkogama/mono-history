using System;

internal class CubeModelTool : IState
{
	protected bool waitForMouseUp;

	private static EditCubeChange cubeChange;

	private static int cubeCount;

	public static Action<int, EditCubeChange> OnEditCubeChange;

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

	protected static void SendCubeEvent(int cubeCount, EditCubeChange cubeChange)
	{
		CubeModelTool.cubeCount = cubeCount;
		CubeModelTool.cubeChange = cubeChange;
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
		if (cubeChange != EditCubeChange.None && OnEditCubeChange != null)
		{
			OnEditCubeChange(cubeCount, cubeChange);
		}
		cubeChange = EditCubeChange.None;
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
