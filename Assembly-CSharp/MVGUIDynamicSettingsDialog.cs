using System.Collections;

public class MVGUIDynamicSettingsDialog : MVGUISettingsDialog
{
	protected Hashtable oldData;

	public MVGUIDynamicSettingsDialog()
	{
		oldData = (Hashtable)wo.Data.Clone();
	}

	protected void OnDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			Hashtable woData = (Hashtable)dialog.GetResult();
			MVGameController.Instance.Game.UpdateWorldObjectDataPartial(wo.Id, woData);
		}
		else
		{
			wo.Data = oldData;
			wo.OnDataUpdate();
		}
	}

	protected void OnIntermediateResult(UXDialogBox dialog)
	{
		Hashtable woData = (Hashtable)dialog.GetResult();
		wo.PartialUpdateWOData(woData);
	}
}
