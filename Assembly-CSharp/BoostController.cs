using System;
using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;
using UnityEngine;

public class BoostController : IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
{
	public Action BoostCountChanged;

	private static readonly Dictionary<BoostType, Boost> boosts = new Dictionary<BoostType, Boost>
	{
		{
			BoostType.AmmoIntMultiplier,
			new Boost(BoostType.AmmoIntMultiplier, "Ammo", TM._("x2 Ammo"), TM._("Ammo Percentage"), TM._("Ammo"), allowedForGame: false, 600f)
		},
		{
			BoostType.MovementSpeedFloatMultiplier,
			new Boost(BoostType.MovementSpeedFloatMultiplier, "Speed", TM._("+{0}% Speed"), TM._("Speed Percentage"), TM._("Speed"), allowedForGame: true, 600f)
		},
		{
			BoostType.GameCoinsIntMultiplier,
			new Boost(BoostType.GameCoinsIntMultiplier, "GameCoinBoost", TM._("x2 Gamecoins"), TM._("Coin Percentage"), TM._("Coins"), allowedForGame: false, 600f)
		},
		{
			BoostType.ExtraHealthFloatMultiplier,
			new Boost(BoostType.ExtraHealthFloatMultiplier, "Health", TM._("+{0}% HP"), TM._("HP Percentage"), TM._("Health"), allowedForGame: true, 600f)
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

	private List<Boost> removeList = new List<Boost>();

	public void Initialize()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void UpdateControllerUpdate()
	{
		foreach (Boost value in activeBoosts.Values)
		{
			value.BoostSecondsLeft -= Time.deltaTime;
			if (value.BoostSecondsLeft <= 0f)
			{
				value.BoostSecondsLeft = value.BoostMaxDurationSeconds;
				removeList.Add(value);
			}
		}
		for (int i = 0; i < removeList.Count; i++)
		{
			activeBoosts.Remove(removeList[i].Type);
			BoostUpdated(removeList[i].Type);
		}
		removeList.Clear();
	}

	public void ActivateBoost(BoostType type)
	{
		if (!IsBoostActive(type))
		{
			activeBoosts[type] = boosts[type];
			activeBoosts[type].BoostSecondsLeft = boosts[type].BoostMaxDurationSeconds;
			BoostUpdated(type);
		}
		else
		{
			Debug.LogWarning("Boost: " + type.ToString() + ". Active boosts: " + activeBoosts.ToString());
			Debug.LogError("Trying to activate boost, but boost is already active.");
		}
	}

	private void BoostUpdated(BoostType type)
	{
		if (onBoostTypeUpdate[type] != null)
		{
			onBoostTypeUpdate[type]();
		}
		if (BoostCountChanged != null)
		{
			BoostCountChanged();
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

	public bool HasAvailableBoosts()
	{
		MVGameBoosterDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameBoosterDataObject>();
		Dictionary<BoostType, Boost>.ValueCollection allBoosts = GetAllBoosts();
		foreach (Boost item in allBoosts)
		{
			List<GameBoosterSettingWithGoldSetting> activeSettingsList = singletonWorldObject.GameBoosterSettingsManager.ActiveSettingsList;
			for (int i = 0; i < activeSettingsList.Count; i++)
			{
				if (item.BoostKey == activeSettingsList[i].Key)
				{
					return true;
				}
			}
		}
		return false;
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
