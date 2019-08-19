using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpawnRoleTierEditorMenu : MonoBehaviour
{
	[SerializeField]
	private Text currentTierNumberText;

	[SerializeField]
	private Text progressBarText;

	[SerializeField]
	private ProgressBar progressBar;

	[SerializeField]
	private SpawnRoleTierSettings tierSettingsPrefab;

	private int spawnRoleCost;

	private GamePassTier currentTier;

	private bool canSelectTier0;

	private UnityAction<GamePassTier> ChangeTierRequirement;

	public void Initialize(GamePassTier newTier, int newspawnRoleCost, UnityAction<GamePassTier> ChangeTierRequirement)
	{
		this.ChangeTierRequirement = ChangeTierRequirement;
		spawnRoleCost = newspawnRoleCost;
		UpdateTier(newTier);
	}

	public void UpdateSpawnRoleCost(int newspawnRoleCost)
	{
		spawnRoleCost = newspawnRoleCost;
		int num = 100;
		if (currentTier == GamePassTier.Tier0)
		{
			float progress = (float)spawnRoleCost / (float)num;
			progressBar.Progress = progress;
			progressBarText.text = spawnRoleCost + "/" + num;
		}
		else
		{
			progressBar.Progress = 1f;
			progressBarText.text = num.ToString();
		}
		canSelectTier0 = spawnRoleCost <= num;
	}

	public void UpdateTier(GamePassTier newTier)
	{
		currentTier = newTier;
		Text text = currentTierNumberText;
		int num = (int)currentTier;
		text.text = num.ToString();
		UpdateSpawnRoleCost(spawnRoleCost);
	}

	public void SelectTier()
	{
		SpawnRoleTierSettings tierSettings = Object.Instantiate(tierSettingsPrefab);
		tierSettings.Initialize(currentTier, canSelectTier0, OnTierSelected);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierSettings.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	private void OnTierSelected(GamePassTier newTier)
	{
		UpdateTier(newTier);
		ChangeTierRequirement(newTier);
	}
}
