using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SpawnRoleTierSettings : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> tierSelectedEffectObjects;

	[SerializeField]
	private GameObject tierZero;

	[SerializeField]
	private GameObject tierZeroGray;

	private bool canSelectTier0;

	private UnityAction<GamePassTier> OnChangeTierCallback;

	public void Initialize(GamePassTier currentTier, bool canSelectTier0, UnityAction<GamePassTier> OnChangeTierCallback)
	{
		this.canSelectTier0 = canSelectTier0;
		this.OnChangeTierCallback = OnChangeTierCallback;
		tierSelectedEffectObjects[(int)currentTier].SetActive(value: true);
		tierZero.SetActive(canSelectTier0);
		tierZeroGray.SetActive(!canSelectTier0);
	}

	public void OnSelectTier0()
	{
		if (canSelectTier0)
		{
			SelectTier(GamePassTier.Tier0);
		}
	}

	public void OnSelectTier1()
	{
		SelectTier(GamePassTier.Tier1);
	}

	public void OnSelectTier2()
	{
		SelectTier(GamePassTier.Tier2);
	}

	public void OnSelectTier3()
	{
		SelectTier(GamePassTier.Tier3);
	}

	private void SelectTier(GamePassTier newTier)
	{
		OnChangeTierCallback(newTier);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
