using System.Collections.Generic;

public class MVGUIDynamicSettingsDialog : MVGUISettingsDialog
{
	protected Dictionary<object, object> oldData;

	public MVGUIDynamicSettingsDialog()
	{
		oldData = new Dictionary<object, object>(wo.Data);
	}

	protected void OnDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			Dictionary<object, object> woData = (Dictionary<object, object>)dialog.GetResult();
			MVGameControllerBase.Game.UpdateWorldObjectDataPartial(wo.Id, woData);
		}
		else
		{
			wo.Data = oldData;
			wo.OnDataUpdate();
		}
	}

	protected void OnIntermediateResult(UXDialogBox dialog)
	{
		Dictionary<object, object> woData = (Dictionary<object, object>)dialog.GetResult();
		wo.PartialUpdateWOData(woData);
	}
}
