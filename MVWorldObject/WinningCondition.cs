using System;

public abstract class WinningCondition : IWinningCondition
{
	private bool forfilled;

	private readonly int id;

	private int limit;

	private OnCounterTypeChangedArgs instigatorCounterTypeChangedEvent;

	private readonly GameStatCounterType gameStatCounterType;

	protected WinningConditionPresentStyle winningConditionPresentStyle;

	protected readonly GameStatCounterManager gameCounterManager;

	public int ID => id;

	public IWinningCondition Parent { get; private set; }

	public int Limit => limit;

	public bool Forfilled => forfilled;

	public OnCounterTypeChangedArgs InstigatorCounterTypeChangedEvent
	{
		get
		{
			if (!forfilled)
			{
				throw new Exception("WinningCondition not forfilled so InstigatorCounterTypeChangedEvent not yet created");
			}
			return instigatorCounterTypeChangedEvent;
		}
	}

	public virtual bool IsBriefingNode { get; private set; }

	public virtual bool IsTeamMode => gameCounterManager.ActiveTeams.Count > 1;

	public virtual HighScores HighScores => gameCounterManager.GetHighScores(GameStatCounterType, gameCounterManager.ActiveTeams.Count > 1, WinningConditionPresentStyle.MultipleWinners, byAscending: false);

	public GameStatCounterType GameStatCounterType => gameStatCounterType;

	public WinningConditionPresentStyle WinningConditionPresentStyle => winningConditionPresentStyle;

	public abstract bool IsSingleton { get; }

	public event EventHandler<EventArgs> OnWinningConditionChanged;

	public virtual bool Traverse(Func<IWinningCondition, bool> callBack)
	{
		return callBack(this);
	}

	public void SetLimit(int limit)
	{
		this.limit = limit;
		if (OnWinningConditionChanged != null)
		{
			OnWinningConditionChanged(this, null);
		}
	}

	public override string ToString()
	{
		return $"WinningConditionType: {GetType()}. Forfilled: {Forfilled}. Limit: {Limit}. \nInstigatorEvent: {instigatorCounterTypeChangedEvent}";
	}

	protected WinningCondition(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, int limit, bool isBriefingNode, GameStatCounterType gameStatCounterType, WinningConditionPresentStyle winningConditionPresentStyle)
	{
		this.gameCounterManager = gameCounterManager;
		this.id = id;
		Parent = parent;
		this.limit = limit;
		IsBriefingNode = isBriefingNode;
		this.gameStatCounterType = gameStatCounterType;
		this.winningConditionPresentStyle = winningConditionPresentStyle;
		this.gameCounterManager.OnCounterTypeChanged += GameCountersQuery_OnCounterTypeChangedGameStatCounterTypeOnly;
	}

	public virtual void Reset()
	{
		forfilled = false;
		instigatorCounterTypeChangedEvent = null;
	}

	private void GameCountersQuery_OnCounterTypeChangedGameStatCounterTypeOnly(object sender, OnCounterTypeChangedArgs e)
	{
		if (e.counterType == GameStatCounterType)
		{
			GameCountersQuery_OnCounterTypeChanged(sender, e);
		}
	}

	protected virtual void GameCountersQuery_OnCounterTypeChanged(object sender, OnCounterTypeChangedArgs e)
	{
	}

	protected void SendWinningConditionChangedEvent(EventArgs eventArgs, OnCounterTypeChangedArgs instigatorCounterTypeChangedEvent)
	{
		forfilled = true;
		this.instigatorCounterTypeChangedEvent = instigatorCounterTypeChangedEvent;
		if (OnWinningConditionChanged != null)
		{
			OnWinningConditionChanged(this, eventArgs);
		}
	}
}
