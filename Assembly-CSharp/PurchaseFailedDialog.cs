public static class PurchaseFailedDialog
{
	public static void ShowErrorDialog(int returnCode, int priceGold, int priceSilver)
	{
		if (returnCode == 1)
		{
			if (priceGold > 0 && priceSilver == 0)
			{
				UXUtils.UXDialogFactory.CreateDialog(TM._("Get more gold?"), TM._("Insufficient Funds"), UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
					.SetOnResultCallback(OnGoldPurchaseDialogResult)
					.Show();
			}
			else if (priceGold == 0 && priceSilver > 0)
			{
				UXUtils.UXDialogFactory.CreateDialog(TM._("Get more silver"), TM._("Insufficient Funds"), UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
					.SetOnResultCallback(OnSilverConvertDialogResult)
					.Show();
			}
			else
			{
				UXUtils.UXDialogFactory.CreateDialog(TM._("Get more gold?"), TM._("Insufficient Funds"), UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
					.SetOnResultCallback(OnGoldPurchaseDialogResult)
					.Show();
			}
		}
		else
		{
			UXUtils.UXDialogFactory.CreateDialog(TM._("An error occured:\n\n"), TM._("Error"), UXDialogType.Simple, noButtons: false, stackDialog: true, canClose: true, new ValueInsert().AddInt(returnCode)).Show();
		}
	}

	private static void OnSilverConvertDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult != UXDialogResult.Positive)
		{
		}
	}

	private static void OnGoldPurchaseDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoPurchaseGold");
			BrowserComm.ExecuteBrowserRequest(MVGameController.GameSessionData.purchaseGoldURL);
		}
	}
}
