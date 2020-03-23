using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class TierUnlockedPopupContentSpawnRole : TierUnlockedPopupContentBase
{
	[SerializeField]
	private GamePassesShopContentCuller contentCuller;

	public override void Initialize(GamePassTier unlockedGamePassTier, UnityAction onDisplayDoneCallback)
	{
		base.Initialize(unlockedGamePassTier, onDisplayDoneCallback);
		contentCuller.Initialize();
	}

	protected override void HandleDisplaying()
	{
		HandleSlideTitleText();
	}

	public void AddSpawnRoleRewardInfo(GamePassesSpawnRoleRewardInfo spawnRoleInfo)
	{
		spawnRoleInfo.transform.SetParent(mainContent, worldPositionStays: false);
		spawnRoleInfo.gameObject.SetActive(value: true);
		contentCuller.AddContentElement(spawnRoleInfo);
	}

	public void OnContinuePressed()
	{
		onDisplayDoneCallback();
	}
}
