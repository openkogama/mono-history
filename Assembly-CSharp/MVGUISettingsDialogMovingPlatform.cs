using System.Collections.Generic;
using UnityEngine;

public class MVGUISettingsDialogMovingPlatform : MVGUISettingsDialog
{
	public MVGUISettingsDialogMovingPlatform()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/MovableSpeedSettingsDialog", TM._("Moving Platform")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private void OnDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			float num = (float)dialog.GetResult();
			MVMovingPlatformGroup mVMovingPlatformGroup = wo as MVMovingPlatformGroup;
			MVMovingPlatform platform = mVMovingPlatformGroup.Platform;
			Vector3 normalized = platform.Velocity.normalized;
			float num2 = num;
			Vector3 vec = normalized * num2;
			string keyPath = "BlueprintData\\Velocity";
			MVGameControllerBase.Game.UpdateWorldObjectDataPartial(platform.Id, keyPath, vec.ToSerializeString());
		}
		MVGameControllerLegacyUI.EditorController.EditorStateMachine.DeSelectAll();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		MVMovingPlatformGroup mVMovingPlatformGroup = wo as MVMovingPlatformGroup;
		MVMovingPlatform platform = mVMovingPlatformGroup.Platform;
		dictionary.Add("SpeedSlider", new SliderData
		{
			sliderValue = platform.Velocity.magnitude,
			setMinMaxValue = true,
			minValue = 0.3f,
			maxValue = 3f
		});
		return dictionary;
	}
}
