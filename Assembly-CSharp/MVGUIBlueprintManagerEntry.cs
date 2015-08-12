using System;
using UnityEngine;

public class MVGUIBlueprintManagerEntry : UXCustomDialogBox
{
	public UXTextButton createButton;

	public UXTextButton openExisting;

	private bool _isInitialized;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXTextButton uXTextButton = openExisting;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OpenExistingBlueprint();
			}));
			UXTextButton uXTextButton2 = createButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OpenBluePrintOverview(-1);
			}));
			_isInitialized = true;
		}
	}

	private void OpenExistingBlueprint()
	{
		(UXUtils.FindComponentInParents(typeof(UXView), transform.parent) as UXView).ReleaseFocus();
		OnPositiveClose();
		DialogFactory.CloseDialog();
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/BlueprintManagerPickExisting", "Pick Existing", noButtons: true).Show();
	}

	private void OpenBluePrintOverview(int woID)
	{
		Debug.Log("picked id: " + woID);
		(UXUtils.FindComponentInParents(typeof(UXView), transform.parent) as UXView).ReleaseFocus();
		OnPositiveClose();
		DialogFactory.CloseDialog();
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/BlueprintManagerOverview", "Blueprint Manager", noButtons: true).Show();
		(DialogFactory.CurrentDialogBox as MVGUIBlueprintManagerOverview).SetWoid(woID);
	}
}
