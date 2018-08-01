using System;
using System.Collections.Generic;

public class WinningConditionManager
{
	private class ForfilledWinnerConditionGenerator
	{
		public readonly List<IWinningCondition> gameWonWinnerConditions = new List<IWinningCondition>();

		public ForfilledWinnerConditionGenerator(WinningConditionManager winnerConditionManager)
		{
			winnerConditionManager.Traverse(HandleWinningConditionForReport);
		}

		private bool HandleWinningConditionForReport(IWinningCondition winnerCondition)
		{
			if (winnerCondition.Forfilled)
			{
				Report(winnerCondition);
			}
			return false;
		}

		private bool Report(IWinningCondition winnerCondition)
		{
			if (!(winnerCondition is WinningConditionGroup))
			{
				IWinningCondition parent = winnerCondition.Parent;
				bool result = parent is WinningConditionOr;
				gameWonWinnerConditions.Add(winnerCondition);
				return result;
			}
			return false;
		}
	}

	private WinningConditionOr winnerConditionsRoot;

	private int winnerConditionIDCounter;

	private GameStatCounterManager gameCounterManager;

	public bool WinningConditionFound { get; private set; }

	public event EventHandler<EventArgs> OnWinningConditionChanged;

	public event EventHandler<EventArgs> OnWinningConditionReset;

	public event EventHandler<EventArgs> OnWinningConditionAddedOrRemoved;

	public event EventHandler<EventArgs> OnWinningConditionStateChangedEditMode;

	public void Initialize(GameStatCounterManager gameStatCounterManager)
	{
		gameCounterManager = gameStatCounterManager;
		winnerConditionsRoot = CreateInstance<WinningConditionOr>(null, new object[3]
		{
			false,
			GameStatCounterType.None,
			WinningConditionPresentStyle.NoWinner
		});
		winnerConditionsRoot.OnWinningConditionChanged += winnerConditionsRoot_OnWinningConditionChanged;
	}

	public void Reset()
	{
		WinningConditionFound = false;
		Traverse((IWinningCondition winnerCondition) =>
		{
			winnerCondition.Reset();
			return false;
		});
		gameCounterManager.Clear();
		if (OnWinningConditionReset != null)
		{
			OnWinningConditionReset(this, EventArgs.Empty);
		}
	}

	public void Traverse(Func<IWinningCondition, bool> callBack)
	{
		winnerConditionsRoot.Traverse(callBack);
	}

	public void PublishWinningConditionLimitChanged()
	{
		if (OnWinningConditionStateChangedEditMode != null)
		{
			OnWinningConditionStateChangedEditMode(this, null);
		}
	}

	public List<IWinningCondition> GetForfilledWinningConditions()
	{
		return new ForfilledWinnerConditionGenerator(this).gameWonWinnerConditions;
	}

	public T CreateWinnerCondition<T>(params object[] args) where T : WinningCondition
	{
		return CreateWinnerConditionWithParent<T>(winnerConditionsRoot, args);
	}

	public T CreateWinnerConditionWithParent<T>(WinningCondition parent, params object[] args) where T : WinningCondition
	{
		T singletonWinnerConditionByType = GetSingletonWinnerConditionByType<T>();
		if (singletonWinnerConditionByType != null && singletonWinnerConditionByType.IsSingleton)
		{
			return singletonWinnerConditionByType;
		}
		singletonWinnerConditionByType = CreateInstance<T>(parent, args);
		AddWinnerConditionToNode(parent, singletonWinnerConditionByType);
		if (OnWinningConditionAddedOrRemoved != null)
		{
			OnWinningConditionAddedOrRemoved(this, new EventArgs());
		}
		return singletonWinnerConditionByType;
	}

	public void RemoveWinnerCondition(int id)
	{
		winnerConditionsRoot.RemoveWinnerCondition(id);
		if (OnWinningConditionAddedOrRemoved != null)
		{
			OnWinningConditionAddedOrRemoved(this, new EventArgs());
		}
	}

	public void SetLimitForSingletonWinningConditionWithRoundReset<T>(int limit) where T : WinningCondition
	{
		List<T> winnerConditionsByType = GetWinnerConditionsByType<T>();
		if (winnerConditionsByType.Count != 0)
		{
			if (winnerConditionsByType.Count > 1)
			{
				throw new Exception($"Singleton count of type:{typeof(T)} is: {winnerConditionsByType.Count}");
			}
			T val = winnerConditionsByType[0];
			if (!val.IsSingleton)
			{
				throw new Exception($"Type is not singleton:{typeof(T)}");
			}
			T val2 = winnerConditionsByType[0];
			val2.SetLimit(limit);
			PublishWinningConditionLimitChanged();
		}
	}

	public List<T> GetWinnerConditionsByType<T>() where T : WinningCondition
	{
		List<T> winnerConditionsResult = new List<T>();
		winnerConditionsRoot.Traverse((IWinningCondition winnerCondtion) =>
		{
			if (winnerCondtion.GetType() == typeof(T))
			{
				winnerConditionsResult.Add((T)winnerCondtion);
				if (winnerCondtion.IsSingleton)
				{
					return true;
				}
			}
			return false;
		});
		return winnerConditionsResult;
	}

	public T GetSingletonWinnerConditionByType<T>() where T : WinningCondition
	{
		List<T> winnerConditionsByType = GetWinnerConditionsByType<T>();
		if (winnerConditionsByType.Count == 0)
		{
			return null;
		}
		if (winnerConditionsByType.Count > 1)
		{
			throw new Exception($"Singleton count of type:{typeof(T)} is: {winnerConditionsByType.Count}");
		}
		T val = winnerConditionsByType[0];
		if (!val.IsSingleton)
		{
			throw new Exception($"Type is not singleton:{typeof(T)}");
		}
		return winnerConditionsByType[0];
	}

	public WinningCondition GetWinnerConditionByID(int id)
	{
		WinningCondition winnerConditionBookkeeping = null;
		winnerConditionsRoot.Traverse((IWinningCondition returnWinnerCondition) =>
		{
			if (returnWinnerCondition.ID == id)
			{
				winnerConditionBookkeeping = (WinningCondition)returnWinnerCondition;
				return true;
			}
			return false;
		});
		return winnerConditionBookkeeping;
	}

	public T GetWinnerConditionByTypeAndID<T>(int id) where T : WinningCondition
	{
		WinningCondition winnerConditionByID = GetWinnerConditionByID(id);
		return (T)winnerConditionByID;
	}

	public override string ToString()
	{
		string text = $"WinningCondition found: {WinningConditionFound}.\n";
		return text + $"To win: {winnerConditionsRoot}";
	}

	private void AddWinnerConditionToNode(WinningCondition parent, WinningCondition winnerCondition)
	{
		if (parent == null)
		{
			throw new KeyNotFoundException("Could not find winner condition group with ID");
		}
		if (!(parent is WinningConditionGroup))
		{
			throw new Exception("Type not winnerConditionGroup");
		}
		((WinningConditionGroup)parent).AddWinnerCondition(winnerCondition);
	}

	private T CreateInstance<T>(WinningCondition parent, params object[] args) where T : WinningCondition
	{
		int num = winnerConditionIDCounter++;
		List<object> list = new List<object>();
		list.Add(parent);
		list.Add(num);
		list.Add(gameCounterManager);
		list.AddRange(args);
		return Factory<T>(list.ToArray());
	}

	private void winnerConditionsRoot_OnWinningConditionChanged(object sender, EventArgs eventArgs)
	{
		if (OnWinningConditionChanged != null)
		{
			OnWinningConditionChanged(this, eventArgs);
		}
		WinningConditionFound = true;
	}

	protected virtual T Factory<T>(params object[] args) where T : WinningCondition
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(AllCollectiblesCollected))
		{
			return (T)(WinningCondition)new AllCollectiblesCollected((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		if (typeFromHandle == typeof(WinningConditionOr))
		{
			return (T)(WinningCondition)new WinningConditionOr((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2], (bool)args[3], (GameStatCounterType)args[4], (WinningConditionPresentStyle)args[5]);
		}
		if (typeFromHandle == typeof(WinningConditionAnd))
		{
			return (T)(WinningCondition)new WinningConditionAnd((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		if (typeFromHandle == typeof(CaptureTheFlag))
		{
			return (T)(WinningCondition)new CaptureTheFlag((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		if (typeFromHandle == typeof(FlagReached))
		{
			return (T)(WinningCondition)new FlagReached((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		if (typeFromHandle == typeof(KillLimit))
		{
			return (T)(WinningCondition)new KillLimit((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2], (int)args[3]);
		}
		if (typeFromHandle == typeof(OculusKillLimit))
		{
			return (T)(WinningCondition)new OculusKillLimit((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2], (int)args[3]);
		}
		if (typeFromHandle == typeof(TargetAssasinated))
		{
			return (T)(WinningCondition)new TargetAssasinated((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2], (int)args[3], (int)args[4]);
		}
		if (typeFromHandle == typeof(TargetAssasinatedGroup))
		{
			return (T)(WinningCondition)new TargetAssasinatedGroup((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		if (typeFromHandle == typeof(TimeLimit))
		{
			return (T)(WinningCondition)new TimeLimit((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		if (typeFromHandle == typeof(TimeAttackFlagReached))
		{
			return (T)(WinningCondition)new TimeAttackFlagReached((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		return null;
	}

	public bool CanPlaceWinningCondition<T>() where T : WinningCondition
	{
		if (winnerConditionsRoot.Length <= 0)
		{
			return true;
		}
		if (GetSingletonWinnerConditionByType<T>() != null)
		{
			return true;
		}
		if (winnerConditionsRoot.Length == 1 && winnerConditionsRoot.Traverse(IsTimeLimit))
		{
			return true;
		}
		return false;
	}

	private bool IsTimeLimit(IWinningCondition winningCondition)
	{
		if (winningCondition is TimeLimit)
		{
			return true;
		}
		return false;
	}
}
