using System;
using UnityEngine.EventSystems;

public interface IBoostAdController : IEventSystemHandler
{
	void TryShowAd(BoostType type, Action<bool> OnUnlockedCallback);

	void TryShowAdForReboost(BoostType type, Action<bool> OnUnlockedCallback);
}
