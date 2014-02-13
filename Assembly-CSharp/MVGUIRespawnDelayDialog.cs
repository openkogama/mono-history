using MV.Common;
using UnityEngine;

public class MVGUIRespawnDelayDialog : UXCustomDialogBox
{
	private const float RESPAWN_DELAY_TIME = 10f;

	public UXText countdownText;

	public UXGroup respawnNowGroup;

	public UXGroup priceGroup;

	public UXText priceText;

	public UXGroup noFundsGroup;

	private float _startTime;

	private bool _purchasingRespawn;

	private AIngameController IngameController => MVGameController.Instance.IngameController;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		priceText.Text = CommonValues.RespawnNowPrice.silver + string.Empty;
		_startTime = Time.time;
	}

	private void Update()
	{
		int num = (int)Mathf.Ceil(10f - (Time.time - _startTime));
		countdownText.Text = ((num >= 10) ? string.Empty : "0") + num;
		if (Time.time - _startTime > 10f)
		{
			DoRespawn();
		}
		bool flag = IngameController.IsChatShown() && IngameController.ChatHasFocus();
		if (!_purchasingRespawn && !flag && Input.GetKeyDown((KeyCode)107))
		{
			TryPurchaseRespawn();
		}
	}

	private void TryPurchaseRespawn()
	{
		_purchasingRespawn = true;
		if (MVGameController.Instance.Game.LocalPlayer.GoldAmount >= CommonValues.RespawnNowPrice.gold && MVGameController.Instance.Game.LocalPlayer.SilverAmount >= CommonValues.RespawnNowPrice.silver)
		{
			MVGameController.Instance.Game.PurchaseRespawnNow();
			PurchaseRespawn(success: true);
		}
		else
		{
			PurchaseRespawn(success: false);
		}
	}

	private void PurchaseRespawn(bool success)
	{
		if (success)
		{
			DoRespawn();
			return;
		}
		respawnNowGroup.Hide();
		priceGroup.Hide();
		noFundsGroup.Show();
	}

	private void DoRespawn()
	{
		OnPositiveClose();
		DialogFactory.CloseDialog();
	}
}
