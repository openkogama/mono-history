using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class SpawnRoleSelectionButtonController : MonoBehaviour
{
	[SerializeField]
	private GameObject selectButton;

	[SerializeField]
	private GameObject buyTierButton;

	[SerializeField]
	private GameObject lockedTierButton;

	[SerializeField]
	private Text buyTierButtonText;

	[SerializeField]
	private Text lockedTierButtonText;

	[SerializeField]
	private GameObject FreeTryUI;

	private GamePassTier currentSpawnRoleGamePassTier;

	public void OnNewSelectedSpawnRole(GamePassTier spawnRoleTier)
	{
		currentSpawnRoleGamePassTier = spawnRoleTier;
		if (GamePassesManager.PlayerPlanetData != null)
		{
			GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
			GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
			bool active = spawnRoleTier == previewGamePassTier && spawnRoleTier != GamePassTier.Tier0;
			FreeTryUI.SetActive(active);
			if ((int)spawnRoleTier <= (int)gamePassTier || (int)spawnRoleTier <= (int)previewGamePassTier)
			{
				selectButton.SetActive(value: true);
				buyTierButton.SetActive(value: false);
				lockedTierButton.SetActive(value: false);
			}
			else if (spawnRoleTier == gamePassTier + 1)
			{
				selectButton.SetActive(value: false);
				buyTierButton.SetActive(value: true);
				lockedTierButton.SetActive(value: false);
				buyTierButtonText.text = "Unlock Tier " + (int)spawnRoleTier;
			}
			else
			{
				selectButton.SetActive(value: false);
				buyTierButton.SetActive(value: false);
				lockedTierButton.SetActive(value: true);
				lockedTierButtonText.text = "Tier " + (int)spawnRoleTier + " Locked";
			}
		}
	}

	private void OnEnable()
	{
		OnNewSelectedSpawnRole(currentSpawnRoleGamePassTier);
	}
}
