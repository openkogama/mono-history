using UnityEngine;

public class MVGUIGameState : UXViewScript
{
	[SerializeField]
	private UXText itemMsg;

	[SerializeField]
	private UXBucket itemIcon;

	private void Update()
	{
		AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameController.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		if (singletonWinnerConditionByType != null)
		{
			string text = MVGameController.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible) + "/" + singletonWinnerConditionByType.Limit;
			if (text != itemMsg.Text)
			{
				itemMsg.Text = text;
			}
			if (View.isVisible)
			{
				itemMsg.SetVisible(visible: true);
				itemIcon.SetVisible(visible: true);
			}
		}
		else
		{
			itemMsg.Text = string.Empty;
			itemMsg.SetVisible(visible: false);
			itemIcon.SetVisible(visible: false);
		}
	}
}
