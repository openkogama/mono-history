using System;
using System.Collections.Generic;
using UnityEngine;

public class BoostController
{
	public Action<int> BoostCountChanged;

	private static readonly Dictionary<BoostType, Boost> boosts = new Dictionary<BoostType, Boost>
	{
		{
			BoostType.AmmoIntMultiplier,
			new Boost(BoostType.AmmoIntMultiplier, TM._("x2 Ammo"), allowedForGame: false, 2)
		},
		{
			BoostType.MovementSpeedFloatMultiplier,
			new Boost(BoostType.MovementSpeedFloatMultiplier, string.Format(TM._("+{0}% Speed"), 10), allowedForGame: true, 1.1f)
		},
		{
			BoostType.GameCoinsIntMultiplier,
			new Boost(BoostType.GameCoinsIntMultiplier, TM._("x2 Gamecoins"), allowedForGame: false, 2)
		},
		{
			BoostType.ExtraHealthFloatMultiplier,
			new Boost(BoostType.ExtraHealthFloatMultiplier, string.Format(TM._("+{0}% HP"), 50), allowedForGame: true, 1.5f)
		}
	};

	private Dictionary<BoostType, Action> onBoostTypeUpdate = new Dictionary<BoostType, Action>
	{
		{
			BoostType.AmmoIntMultiplier,
			null
		},
		{
			BoostType.MovementSpeedFloatMultiplier,
			null
		},
		{
			BoostType.GameCoinsIntMultiplier,
			null
		},
		{
			BoostType.ExtraHealthFloatMultiplier,
			null
		}
	};

	private Dictionary<BoostType, Boost> activeBoosts = new Dictionary<BoostType, Boost>();

	public void ActivateBoost(BoostType type)
	{
		if (!IsBoostActive(type))
		{
			activeBoosts[type] = boosts[type];
			if (onBoostTypeUpdate[type] != null)
			{
				onBoostTypeUpdate[type]();
			}
			if (BoostCountChanged != null)
			{
				BoostCountChanged(activeBoosts.Count);
			}
		}
		else
		{
			Debug.LogWarning("Boost: " + type.ToString() + ". Active boosts: " + activeBoosts.ToString());
			Debug.LogError("Trying to activate boost, but boost is already active.");
		}
	}

	public void AllowBoost(BoostType boost, bool allowed)
	{
		if (boosts.ContainsKey(boost))
		{
			boosts[boost].AllowedForGame = allowed;
		}
		if (activeBoosts.ContainsKey(boost))
		{
			activeBoosts[boost].AllowedForGame = allowed;
		}
	}

	public void SubscribeToBoostChanged(BoostType type, Action callback)
	{
		Dictionary<BoostType, Action> dictionary;
		BoostType key;
		(dictionary = onBoostTypeUpdate)[key = type] = (Action)Delegate.Combine(dictionary[key], callback);
	}

	public void UnSubscribeToBoostChanged(BoostType type, Action callback)
	{
		Dictionary<BoostType, Action> dictionary;
		BoostType key;
		(dictionary = onBoostTypeUpdate)[key = type] = (Action)Delegate.Remove(dictionary[key], callback);
	}

	public bool IsBoostActive(BoostType type)
	{
		return activeBoosts.ContainsKey(type);
	}

	public Dictionary<BoostType, Boost>.ValueCollection GetAllBoosts()
	{
		return boosts.Values;
	}

	public Dictionary<BoostType, Boost>.ValueCollection GetActiveBoosts()
	{
		return activeBoosts.Values;
	}

	public bool TryGetActiveBoost(BoostType type, out Boost boost)
	{
		if (activeBoosts.ContainsKey(type))
		{
			boost = activeBoosts[type];
			return true;
		}
		boost = null;
		return false;
	}

	public bool TryGetBoost(BoostType type, out Boost boost)
	{
		if (boosts.ContainsKey(type))
		{
			boost = boosts[type];
			return true;
		}
		boost = null;
		return false;
	}
}
