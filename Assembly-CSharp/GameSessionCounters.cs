using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public static class GameSessionCounters
{
	public class GameSessionCounter
	{
		private ObscuredInt count;

		private readonly GameSessionCounterType gameSessionCounterType;

		public Action<GameSessionCounterType, int> callbacks;

		public GameSessionCounter(GameSessionCounterType gameSessionCounterType)
		{
			this.gameSessionCounterType = gameSessionCounterType;
		}

		public void Increment()
		{
			++count;
			Notify();
		}

		public void Decrement()
		{
			--count;
			Notify();
		}

		public void SetCount(int count)
		{
			this.count = count;
			Notify();
		}

		public void Add(int value)
		{
			count = (int)count + value;
			Notify();
		}

		private void Notify()
		{
			if (callbacks != null)
			{
				callbacks(gameSessionCounterType, count);
			}
		}
	}

	private static Dictionary<GameSessionCounterType, GameSessionCounter> gameSessionCounters = Setup();

	public static void Increment(GameSessionCounterType gameSessionCounterType)
	{
		gameSessionCounters[gameSessionCounterType].Increment();
	}

	public static void Add(GameSessionCounterType gameSessionCounterType, int value)
	{
		gameSessionCounters[gameSessionCounterType].Add(value);
	}

	public static void Decrement(GameSessionCounterType gameSessionCounterType)
	{
		gameSessionCounters[gameSessionCounterType].Decrement();
	}

	public static void SetCount(GameSessionCounterType gameSessionCounterType, int count)
	{
		gameSessionCounters[gameSessionCounterType].SetCount(count);
	}

	private static Dictionary<GameSessionCounterType, GameSessionCounter> Setup()
	{
		Dictionary<GameSessionCounterType, GameSessionCounter> dictionary = new Dictionary<GameSessionCounterType, GameSessionCounter>();
		AddSessionCounters(dictionary);
		GameSessionCounterRules.AddRules(dictionary);
		return dictionary;
	}

	private static void AddSessionCounters(Dictionary<GameSessionCounterType, GameSessionCounter> sessionCounters)
	{
		foreach (int value in Enum.GetValues(typeof(GameSessionCounterType)))
		{
			sessionCounters.Add((GameSessionCounterType)value, new GameSessionCounter((GameSessionCounterType)value));
		}
	}
}
