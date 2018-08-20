using System;

public class WinningConditionManagerClient : WinningConditionManager
{
	protected override T Factory<T>(object[] args)
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(AllCollectiblesCollectedClient))
		{
			return (T)(WinningCondition)new AllCollectiblesCollectedClient((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		if (typeFromHandle == typeof(FlagReachedClient))
		{
			return (T)(WinningCondition)new FlagReachedClient((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		if (typeFromHandle == typeof(KillLimitClient))
		{
			return (T)(WinningCondition)new KillLimitClient((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2], (int)args[3]);
		}
		if (typeFromHandle == typeof(OculusKillLimitClient))
		{
			return (T)(WinningCondition)new OculusKillLimitClient((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2], (int)args[3]);
		}
		if (typeFromHandle == typeof(TimeLimitClient))
		{
			return (T)(WinningCondition)new TimeLimitClient((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		if (typeFromHandle == typeof(TimeAttackFlagReachedClient))
		{
			return (T)(WinningCondition)new TimeAttackFlagReachedClient((WinningCondition)args[0], (int)args[1], (GameStatCounterManager)args[2]);
		}
		return base.Factory<T>(args);
	}
}
